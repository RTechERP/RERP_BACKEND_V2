using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM.PinAuth;
using RERPAPI.Repo.GenericEntity;
using System.Text.RegularExpressions;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class PinAuthController : ControllerBase
    {
        private readonly CurrentUser _currentUser;
        private readonly EmailHelper _emailHelper;
        private readonly EmployeeRepo _employeeRepo;
        private readonly UserRepo _userRepo;
        private readonly PinResetTokenRepo _pinResetTokenRepo;

        public PinAuthController(
            CurrentUser currentUser, 
            EmailHelper emailHelper, 
            EmployeeRepo employeeRepo,
            UserRepo userRepo,
            PinResetTokenRepo pinResetTokenRepo)
        {
            _currentUser = currentUser;
            _emailHelper = emailHelper;
            _employeeRepo = employeeRepo;
            _userRepo = userRepo;
            _pinResetTokenRepo = pinResetTokenRepo;
        }

        /// <summary>
        /// Check xem tài khoản có mã pin cjhua
        /// </summary>
        [Authorize]
        [HttpGet("check-pin-status")]
        public IActionResult CheckPinStatus()
        {
            try
            {
                int userId = _currentUser.ID;
                var user = _userRepo.GetByID(userId);
                if (user == null || user.ID == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy tài khoản"));

                bool hasPIN = !string.IsNullOrEmpty(user.PinPassword);
                return Ok(ApiResponseFactory.Success(new { hasPin = hasPIN }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Tạo Mã PIN lần đầu tiền 
        /// </summary>
        [Authorize]
        [HttpPost("set-pin")]
        public async Task<IActionResult> SetPin([FromBody] SetPinParam param)
        {
            try
            {
                if (string.IsNullOrEmpty(param.Pin) || string.IsNullOrEmpty(param.ConfirmPin))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập đầy đủ PIN"));

                if (!Regex.IsMatch(param.Pin, @"^\d{6}$"))
                    return BadRequest(ApiResponseFactory.Fail(null, "PIN phải gồm đúng 6 chữ số"));

                if (param.Pin != param.ConfirmPin)
                    return BadRequest(ApiResponseFactory.Fail(null, "PIN xác nhận không khớp"));

                int userId = _currentUser.ID;
                var user = _userRepo.GetByID(userId);
                if (user == null || user.ID == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy tài khoản"));

                user.PinPassword = MaHoaMD5.Hash(param.Pin);
                await _userRepo.UpdateAsync(user);

                return Ok(ApiResponseFactory.Success(null, "Tạo PIN thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Xác thực mã pin
        /// </summary>
        [Authorize]
        [HttpPost("verify-pin")]
        public IActionResult VerifyPin([FromBody] VerifyPinParam param)
        {
            try
            {
                if (string.IsNullOrEmpty(param.Pin))
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập PIN"));

                int userId = _currentUser.ID;
                var user = _userRepo.GetByID(userId);
                if (user == null || user.ID == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy tài khoản"));

                if (string.IsNullOrEmpty(user.PinPassword))
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn chưa thiết lập PIN"));

                string hashed = MaHoaMD5.Hash(param.Pin);
                if (hashed == user.PinPassword)
                    return Ok(ApiResponseFactory.Success(new { verified = true }, "Xác thực thành công"));
                else
                    return Ok(ApiResponseFactory.Fail(null, "Sai PIN"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// gửi meo xác nhận mã pin số (6 chữ số)
        /// </summary>
        [Authorize]
        [HttpPost("request-reset-pin")]
        public async Task<IActionResult> RequestResetPin()
        {
            try
            {
                int userId = _currentUser.ID;
                var user = _userRepo.GetByID(userId);
                if (user == null || user.ID == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy tài khoản"));

                // Invalidate existing unused tokens
                var oldTokens = _pinResetTokenRepo.GetAll(x => x.UserId == userId && x.IsUsed == false);
                foreach (var t in oldTokens)
                    t.IsUsed = true;
                //Sửa lỗi Update Entities
                if (oldTokens.Any())
                {
                    foreach (var item in oldTokens)
                    {
                        item.UpdatedDate = DateTime.Now;
                        await _pinResetTokenRepo.UpdateAsync(item);
                    }
                    
                }    
                    
                // Create new OTP (6 digits)
                string rawToken = new Random().Next(100000, 1000000).ToString();
                var token = new PinResetToken
                {
                    UserId = userId,
                    Token = MaHoaMD5.Hash(rawToken),
                    ExpiredAt = DateTime.Now.AddMinutes(10),
                    IsUsed = false,
                    CreatedDate = DateTime.Now,
                    CreatedBy = _currentUser.LoginName,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = _currentUser.LoginName
                };

                await _pinResetTokenRepo.CreateAsync(token);

                // Get Email from Employee
                var employee = _employeeRepo.GetAll().FirstOrDefault(x => x.UserID == userId);
                string targetEmail = employee?.EmailCongTy;
                if (string.IsNullOrEmpty(targetEmail))
                    targetEmail = employee?.EmailCaNhan;
                
                // Fallback to User email if employee email is missing
                if (string.IsNullOrEmpty(targetEmail))
                    targetEmail = user.Email;

                if (string.IsNullOrEmpty(targetEmail))
                    return BadRequest(ApiResponseFactory.Fail(null, "Tài khoản chưa cấu hình Email (Công ty hoặc Cá nhân). Vui lòng liên hệ quản trị viên."));

                // Send Email
                string subject = "[Lưu ý] Mã xác thực đặt lại mã PIN truy cập Công & Lương";
                string body = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #f0f2f5; padding: 40px 20px;'>
    <div style='max-width: 500px; margin: 0 auto; background-color: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.05);'>
        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center;'>
            <h2 style='color: #4F4F4F; margin: 0; font-size: 22px; font-weight: 700;'>Xác thực mã PIN</h2>
        </div>
        <div style='padding: 40px 30px; color: #333333;'>
            <p style='margin-top: 0; font-size: 16px;'>Chào <b>{user.FullName}</b>,</p>
            <p style='font-size: 15px; line-height: 1.6; color: #555555;'>Bạn vừa yêu cầu đặt lại mã PIN. Vui lòng sử dụng mã xác thực dưới đây để tiếp tục:</p>
            
            <div style='background: #f8f9fa; border: 2px dashed #e9ecef; border-radius: 12px; margin: 30px 0; padding: 30px; text-align: center;'>
                <span style='font-size: 36px; font-weight: 800; color: #764ba2; letter-spacing: 8px; display: block;'>{rawToken}</span>
                <p style='margin-top: 10px; color: #888888; font-size: 13px;'>Mã OTP có hiệu lực trong 10 phút</p>
            </div>
            <p style='font-size: 14px; color: #777777; margin-top: 30px;'>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
        </div>
        <div style='background-color: #f8f9fa; padding: 20px; text-align: center; border-top: 1px solid #eeeeee;'>
            <p style='margin: 0; font-size: 12px; color: #999999;'>© {DateTime.Now.Year} RTech ERP System. All rights reserved.</p>
        </div>
    </div>
</div>";

                await _emailHelper.SendAsync(targetEmail, subject, body);

                return Ok(ApiResponseFactory.Success(null, $"Yêu cầu đặt lại PIN đã được gửi đến email {targetEmail}."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Validate token gửi về
        /// </summary>
        [AllowAnonymous]
        [HttpPost("validate-token")]
        public IActionResult ValidateToken([FromBody] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return BadRequest(ApiResponseFactory.Fail(null, "Token không hợp lệ"));

                string hashedToken = MaHoaMD5.Hash(token);
                var tokenRecord = _pinResetTokenRepo.GetAll(x => x.Token == hashedToken && x.IsUsed == false)
                    .FirstOrDefault();

                if (tokenRecord == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Token không đúng hoặc đã được sử dụng"));

                if (tokenRecord.ExpiredAt < DateTime.Now)
                    return BadRequest(ApiResponseFactory.Fail(null, "Token đã hết hạn"));

                return Ok(ApiResponseFactory.Success(null, "Token hợp lệ"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Đặt lại mã pin
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset-pin")]
        public async Task<IActionResult> ResetPin([FromBody] ResetPinParam param)
        {
            try
            {
                if (string.IsNullOrEmpty(param.Token))
                    return BadRequest(ApiResponseFactory.Fail(null, "Token không hợp lệ"));

                if (string.IsNullOrEmpty(param.NewPin) || !Regex.IsMatch(param.NewPin, @"^\d{6}$"))
                    return BadRequest(ApiResponseFactory.Fail(null, "PIN mới phải gồm đúng 6 chữ số"));

                if (param.NewPin != param.ConfirmPin)
                    return BadRequest(ApiResponseFactory.Fail(null, "PIN xác nhận không khớp"));

                string hashedToken = MaHoaMD5.Hash(param.Token);
                var tokenRecord = _pinResetTokenRepo.GetAll(x => x.Token == hashedToken && x.IsUsed == false)
                    .FirstOrDefault();

                if (tokenRecord == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Token không hợp lệ hoặc đã được sử dụng"));

                if (tokenRecord.ExpiredAt < DateTime.Now)
                    return BadRequest(ApiResponseFactory.Fail(null, "Token đã hết hạn. Vui lòng yêu cầu đặt lại PIN mới"));

                var user = _userRepo.GetByID(tokenRecord.UserId ?? 0);
                if (user == null || user.ID == 0)
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy tài khoản"));

                user.PinPassword = MaHoaMD5.Hash(param.NewPin);
                tokenRecord.IsUsed = true;

                await _userRepo.UpdateAsync(user);
                await _pinResetTokenRepo.UpdateAsync(tokenRecord);

                return Ok(ApiResponseFactory.Success(null, "Đặt lại PIN thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
