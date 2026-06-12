using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities.RTCCourse;
using RERPAPI.Repo.GenericCourseEntity;
using static QRCoder.PayloadGenerator;

namespace RERPAPI.Controllers.CourseWeb
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersWebController : ControllerBase
    {
        private UserRepo _userRepo;
        private EmployeeRepo _employeeRepo;
        public readonly EmailHelper _emailHelper;

        public UsersWebController(UserRepo userRepo, EmployeeRepo employeeRepo, EmailHelper emailHelper)
        {
            _userRepo = userRepo;
            _employeeRepo = employeeRepo;
            _emailHelper = emailHelper;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> getAllData(string? keyword, int? status)
        {
            try
            {
                var data = SQLCourseHelper<object>.ProcedureToList("spGetUsers",
                                            new string[] { "@Keywords", "@Status" },
                                            new object[] { keyword, status });

                return Ok(ApiResponseFactory.Success(SQLCourseHelper<object>.GetListData(data, 0), "Lấy danh sách người dùng thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] User model)
        {
            try
            {
                if (model.ID == 0) // thêm mới
                {
                    if (_userRepo.GetAll(c => c.Email == model.Email).Count() > 0)
                    {
                        return Ok(ApiResponseFactory.Fail(null, "Email đã tồn tại!"));
                    }
                    string hashedPassword = RERPAPI.Model.Common.MaHoaMD5.EncryptPassword(model.PasswordHash);
                    var result = _userRepo.Create(new User
                    {
                        LoginName = model.Email,
                        FullName = model.FullName,
                        BirthOfDate = model.BirthOfDate,
                        PasswordHash = hashedPassword,
                        Email = model.Email,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        Position = model.Position,
                        Organization = model.Organization,
                        Status = model.Status
                    });
                    if (result > 0)
                    {
                        var user = _userRepo.GetAll(c => c.Email == model.Email).FirstOrDefault();
                        var resultEmp = _employeeRepo.Create(new Employee
                        {
                            UserID = user.ID,
                            FullName = model.FullName,
                            BirthOfDate = model.BirthOfDate,
                            Email = model.Email,
                            Status = model.Status
                        });
                        if (resultEmp <= 0)
                        {
                            return Ok(ApiResponseFactory.Fail(null, "Có lỗi xảy ra khi thêm mới người dùng"));
                        }
                    }
                    else
                    {
                        return Ok(ApiResponseFactory.Fail(null, "Có lỗi xảy ra khi thêm mới người dùng"));
                    }
                    return Ok(ApiResponseFactory.Success(model, "Thêm mới người dùng thành công"));
                }
                else // sửa
                {
                    var user = _userRepo.GetByID(model.ID);
                    if (user.ID == 0)
                    {
                        return Ok(ApiResponseFactory.Fail(null, "Không tìm thấy người dùng!"));
                    }
                    user.LoginName = model.Email;
                    user.FullName = model.FullName;
                    user.BirthOfDate = model.BirthOfDate;
                    user.Email = model.Email;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Position = model.Position;
                    user.Organization = model.Organization;
                    user.Status = model.Status;
                    if (_userRepo.Update(user) <= 0)
                    {
                        return Ok(ApiResponseFactory.Fail(null, "Có lỗi xảy ra khi cập nhật người dùng"));
                    }

                    return Ok(ApiResponseFactory.Success(model, "Cập nhật người dùng thành công"));
                }
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-user")]
        public async Task<IActionResult> getUsers([FromQuery] int id)
        {
            try
            {
                var user = _userRepo.GetByID(id);
                if (user.ID == 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Không tìm thấy người dùng!"));
                }
                return Ok(ApiResponseFactory.Success(user, "Lấy dữ liệu người dùng thành công!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivateAsync([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Vui lòng chọn người dùng!"));
                }

                foreach (var id in ids)
                {
                    var user = _userRepo.GetByID(id);

                    if (user != null && user.ID > 0)
                    {
                        user.Status = 0;
                        _userRepo.Update(user);

                        #region body mail

                        string subject = "Tài khoản của bạn đã được kích hoạt";
                        string body = $"""
                                    <!DOCTYPE html>
                                    <html lang="vi">
                                    <head>
                                        <meta charset="UTF-8" />
                                        <title>Tài khoản của bạn đã được kích hoạt</title>
                                    </head>
                                    <body style="margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif;">
                                        <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f6f8; padding:24px 0;">
                                            <tr>
                                                <td align="center">
                                                    <table width="650" cellpadding="0" cellspacing="0" style="background-color:#ffffff; border-radius:8px; overflow:hidden; border:1px solid #e5e7eb;">

                                                        <tr>
                                                            <td style="background-color:#16a34a; padding:18px 24px; color:#ffffff;">
                                                                <h2 style="margin:0; font-size:20px; font-weight:600;">
                                                                    Tài khoản của bạn đã được kích hoạt
                                                                </h2>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td style="padding:24px; color:#333333; font-size:14px; line-height:1.6;">
                                                                <p style="margin-top:0;">Kính gửi <strong>{user.FullName}</strong>,</p>

                                                                <p>
                                                                    Tài khoản của bạn trên hệ thống đã được kích hoạt thành công.
                                                                </p>

                                                                <p>
                                                                    Bạn hiện có thể đăng nhập và sử dụng các chức năng được cấp quyền trên website.
                                                                </p>

                                                                <table width="100%" cellpadding="0" cellspacing="0" style="margin:20px 0; border-collapse:collapse;">
                                                                    <tr>
                                                                        <td style="padding:10px 12px; background-color:#f9fafb; border:1px solid #e5e7eb; font-weight:600; width:180px;">
                                                                            Email đăng nhập
                                                                        </td>
                                                                        <td style="padding:10px 12px; border:1px solid #e5e7eb;">
                                                                            {user.Email}
                                                                        </td>
                                                                    </tr>

                                                                    <tr>
                                                                        <td style="padding:10px 12px; background-color:#f9fafb; border:1px solid #e5e7eb; font-weight:600;">
                                                                            Thời gian kích hoạt
                                                                        </td>
                                                                        <td style="padding:10px 12px; border:1px solid #e5e7eb;">
                                                                            {DateTime.Now:dd/MM/yyyy HH:mm}
                                                                        </td>
                                                                    </tr>
                                                                </table>

                                                               <!-- <div style="margin-top:24px; text-align:center;">
                                                                    <a href="loginUrl"
                                                                       style="display:inline-block; background-color:#16a34a; color:#ffffff; text-decoration:none; padding:10px 20px; border-radius:6px; font-weight:600;">
                                                                        Đăng nhập hệ thống
                                                                    </a>
                                                                </div>-->

                                                                <p style="margin-top:24px;">
                                                                    Trân trọng,<br />
                                                                    <strong>Hệ thống quản lý website</strong>
                                                                </p>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td style="background-color:#f9fafb; padding:14px 24px; color:#6b7280; font-size:12px; text-align:center; border-top:1px solid #e5e7eb;">
                                                                Email này được gửi tự động từ hệ thống. Vui lòng không trả lời email này.
                                                            </td>
                                                        </tr>

                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </body>
                                    </html>
                                    """;

                        #endregion body mail

                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            await _emailHelper.SendRangeAsync(user.Email, subject, body, cc: "");
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(ids, "Kích hoạt người dùng thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateAsync([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Vui lòng chọn người dùng!"));
                }

                foreach (var id in ids)
                {
                    var user = _userRepo.GetByID(id);

                    if (user != null && user.ID > 0)
                    {
                        user.Status = 1;
                        _userRepo.Update(user);

                        #region body mail

                        string subject = "Tài khoản của bạn đã bị khóa";

                        string body = $"""
                                            <!DOCTYPE html>
                                            <html lang="vi">
                                            <head>
                                                <meta charset="UTF-8" />
                                                <title>Tài khoản của bạn đã bị khóa</title>
                                            </head>
                                            <body style="margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif;">
                                                <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f6f8; padding:24px 0;">
                                                    <tr>
                                                        <td align="center">
                                                            <table width="650" cellpadding="0" cellspacing="0" style="background-color:#ffffff; border-radius:8px; overflow:hidden; border:1px solid #e5e7eb;">

                                                                <tr>
                                                                    <td style="background-color:#dc2626; padding:18px 24px; color:#ffffff;">
                                                                        <h2 style="margin:0; font-size:20px; font-weight:600;">
                                                                            Tài khoản của bạn đã bị khóa
                                                                        </h2>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td style="padding:24px; color:#333333; font-size:14px; line-height:1.6;">
                                                                        <p style="margin-top:0;">Kính gửi <strong>{user.FullName}</strong>,</p>

                                                                        <p>
                                                                            Tài khoản của bạn trên hệ thống hiện đã bị khóa.
                                                                        </p>

                                                                        <p>
                                                                            Bạn sẽ tạm thời không thể đăng nhập hoặc sử dụng các chức năng trên website
                                                                            cho đến khi tài khoản được mở khóa.
                                                                        </p>

                                                                        <table width="100%" cellpadding="0" cellspacing="0" style="margin:20px 0; border-collapse:collapse;">
                                                                            <tr>
                                                                                <td style="padding:10px 12px; background-color:#f9fafb; border:1px solid #e5e7eb; font-weight:600; width:180px;">
                                                                                    Email đăng nhập
                                                                                </td>
                                                                                <td style="padding:10px 12px; border:1px solid #e5e7eb;">
                                                                                    {user.Email}
                                                                                </td>
                                                                            </tr>

                                                                            <tr>
                                                                                <td style="padding:10px 12px; background-color:#f9fafb; border:1px solid #e5e7eb; font-weight:600;">
                                                                                    Thời gian khóa
                                                                                </td>
                                                                                <td style="padding:10px 12px; border:1px solid #e5e7eb;">
                                                                                    {DateTime.Now:dd/MM/yyyy HH:mm}
                                                                                </td>
                                                                            </tr>
                                                                        </table>

                                                                        <div style="margin:20px 0; padding:14px 16px; background-color:#fef2f2; border:1px solid #fecaca; border-radius:6px; color:#991b1b;">
                                                                            Vui lòng liên hệ Quản trị viên để được hỗ trợ mở khóa tài khoản.
                                                                        </div>

                                                                        <p style="margin-top:24px;">
                                                                            Trân trọng,<br />
                                                                            <strong>Hệ thống quản lý website</strong>
                                                                        </p>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td style="background-color:#f9fafb; padding:14px 24px; color:#6b7280; font-size:12px; text-align:center; border-top:1px solid #e5e7eb;">
                                                                        Email này được gửi tự động từ hệ thống. Vui lòng không trả lời email này.
                                                                    </td>
                                                                </tr>

                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </body>
                                            </html>
                                            """;

                        #endregion body mail

                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            await _emailHelper.SendRangeAsync(user.Email, subject, body, cc: "");
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(ids, "Khóa người dùng thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("unlock")]
        public async Task<IActionResult> UnlockAsync([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Vui lòng chọn người dùng!"));
                }

                foreach (var id in ids)
                {
                    var user = _userRepo.GetByID(id);

                    if (user != null && user.ID > 0)
                    {
                        user.Status = 0;
                        _userRepo.Update(user);

                        #region body mail

                        string subject = "Tài khoản của bạn đã được mở khóa";

                        string body = $"""
                                        <!DOCTYPE html>
                                        <html lang="vi">
                                        <head>
                                            <meta charset="UTF-8" />
                                            <title>Tài khoản của bạn đã được mở khóa</title>
                                        </head>
                                        <body style="margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif;">
                                            <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f6f8; padding:24px 0;">
                                                <tr>
                                                    <td align="center">
                                                        <table width="650" cellpadding="0" cellspacing="0" style="background-color:#ffffff; border-radius:8px; overflow:hidden; border:1px solid #e5e7eb;">

                                                            <tr>
                                                                <td style="background-color:#2563eb; padding:18px 24px; color:#ffffff;">
                                                                    <h2 style="margin:0; font-size:20px; font-weight:600;">
                                                                        Tài khoản của bạn đã được mở khóa
                                                                    </h2>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td style="padding:24px; color:#333333; font-size:14px; line-height:1.6;">
                                                                    <p style="margin-top:0;">Kính gửi <strong>{user.FullName}</strong>,</p>

                                                                    <p>
                                                                        Tài khoản của bạn trên hệ thống đã được mở khóa thành công.
                                                                    </p>

                                                                    <p>
                                                                        Bạn hiện có thể đăng nhập và tiếp tục sử dụng các chức năng được cấp quyền trên website.
                                                                    </p>

                                                                    <table width="100%" cellpadding="0" cellspacing="0" style="margin:20px 0; border-collapse:collapse;">
                                                                        <tr>
                                                                            <td style="padding:10px 12px; background-color:#f9fafb; border:1px solid #e5e7eb; font-weight:600; width:180px;">
                                                                                Email đăng nhập
                                                                            </td>
                                                                            <td style="padding:10px 12px; border:1px solid #e5e7eb;">
                                                                                {user.Email}
                                                                            </td>
                                                                        </tr>

                                                                        <tr>
                                                                            <td style="padding:10px 12px; background-color:#f9fafb; border:1px solid #e5e7eb; font-weight:600;">
                                                                                Thời gian mở khóa
                                                                            </td>
                                                                            <td style="padding:10px 12px; border:1px solid #e5e7eb;">
                                                                                {DateTime.Now:dd/MM/yyyy HH:mm}
                                                                            </td>
                                                                        </tr>
                                                                    </table>

                                                                    <!--<div style="margin-top:24px; text-align:center;">
                                                                        <a href="loginUrl"
                                                                           style="display:inline-block; background-color:#2563eb; color:#ffffff; text-decoration:none; padding:10px 20px; border-radius:6px; font-weight:600;">
                                                                            Đăng nhập hệ thống
                                                                        </a>
                                                                    </div>-->

                                                                    <p style="margin-top:24px;">
                                                                        Trân trọng,<br />
                                                                        <strong>Hệ thống quản lý website</strong>
                                                                    </p>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td style="background-color:#f9fafb; padding:14px 24px; color:#6b7280; font-size:12px; text-align:center; border-top:1px solid #e5e7eb;">
                                                                    Email này được gửi tự động từ hệ thống. Vui lòng không trả lời email này.
                                                                </td>
                                                            </tr>

                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </body>
                                        </html>
                                        """;

                        #endregion body mail

                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            await _emailHelper.SendRangeAsync(user.Email, subject, body, cc: "");
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(ids, "Mở khóa người dùng thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return Ok(ApiResponseFactory.Fail(null, "Vui lòng chọn người dùng!"));
                }

                foreach (var id in ids)
                {
                    var user = _userRepo.GetByID(id);

                    if (user != null && user.ID > 0)
                    {
                        user.PasswordHash = RERPAPI.Model.Common.MaHoaMD5.EncryptPassword("RTCTechnology");
                        _userRepo.Update(user);

                        #region body mail

                        string subject = "Mật khẩu tài khoản của bạn đã được reset";
                        string body = $""" <!DOCTYPE html> <html lang="vi"> <head> <meta charset="UTF-8" /> <title>Mật khẩu tài khoản của bạn đã được reset</title> </head> <body style="margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif;"> <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f6f8; padding:24px 0;"> <tr> <td align="center"> <table width="650" cellpadding="0" cellspacing="0" style="background-color:#ffffff; border-radius:8px; overflow:hidden; border:1px solid #e5e7eb;"> <tr> <td style="background-color:#f59e0b; padding:18px 24px; color:#ffffff;"> <h2 style="margin:0; font-size:20px; font-weight:600;"> Mật khẩu tài khoản của bạn đã được reset </h2> </td> </tr> <tr> <td style="padding:24px; color:#333333; font-size:14px; line-height:1.6;"> <p style="margin-top:0;">Kính gửi <strong>{user.FullName}</strong>,</p> <p> Tài khoản của bạn trên hệ thống đã được Quản trị viên reset mật khẩu. </p> <p> Bạn có thể sử dụng mật khẩu mới bên dưới để đăng nhập vào hệ thống. </p> <table width="100%" cellpadding="0" cellspacing="0" style="margin:20px 0; border-collapse:collapse;"> <tr> <td style="padding:10px 12px; background-color:#f9fafb; border:1px solid #e5e7eb; font-weight:600; width:180px;"> Email đăng nhập </td> <td style="padding:10px 12px; border:1px solid #e5e7eb;"> {user.Email} </td> </tr> <tr> <td style="padding:10px 12px; background-color:#f9fafb; border:1px solid #e5e7eb; font-weight:600;"> Mật khẩu mới </td> <td style="padding:10px 12px; border:1px solid #e5e7eb;"> <strong style="font-size:16px; color:#b45309;">RTCTechnology</strong> </td> </tr> <tr> <td style="padding:10px 12px; background-color:#f9fafb; border:1px solid #e5e7eb; font-weight:600;"> Thời gian reset </td> <td style="padding:10px 12px; border:1px solid #e5e7eb;"> {DateTime.Now:dd/MM/yyyy HH:mm} </td> </tr> </table> <div style="margin:20px 0; padding:14px 16px; background-color:#fffbeb; border:1px solid #fde68a; border-radius:6px; color:#92400e;"> Vì lý do bảo mật, vui lòng đổi mật khẩu ngay sau khi đăng nhập thành công. </div> <!--<div style="margin-top:24px; text-align:center;"> <a href="loginUrl" style="display:inline-block; background-color:#f59e0b; color:#ffffff; text-decoration:none; padding:10px 20px; border-radius:6px; font-weight:600;"> Đăng nhập hệ thống </a> </div>--> <p style="margin-top:24px;"> Trân trọng,<br /> <strong>Hệ thống quản lý website</strong> </p> </td> </tr> <tr> <td style="background-color:#f9fafb; padding:14px 24px; color:#6b7280; font-size:12px; text-align:center; border-top:1px solid #e5e7eb;"> Email này được gửi tự động từ hệ thống. Vui lòng không trả lời email này. </td> </tr> </table> </td> </tr> </table> </body> </html> """;

                        #endregion body mail

                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            await _emailHelper.SendRangeAsync(user.Email, subject, body, cc: "");
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(ids, "Reset mật khẩu thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}