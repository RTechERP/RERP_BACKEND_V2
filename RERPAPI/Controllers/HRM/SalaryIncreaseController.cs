using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity.HRM;

namespace RERPAPI.Controllers.HRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalaryIncreaseController : ControllerBase
    {
        private readonly SalaryIncreaseRepo _salaryIncreaseRepo;
        private readonly SalaryIncreaseDetailRepo _salaryIncreaseDetailRepo;
        private readonly CurrentUser _currentUser;
        private readonly EmailHelper _emailHelper;
        //private readonly IEmailQueueService _emailQueue;
        private readonly SalaryIncreaseMailSettings _salaryIncreaseMailSettings;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SalaryIncreaseController> _logger;

        public SalaryIncreaseController(
            SalaryIncreaseRepo salaryIncreaseRepo,
            SalaryIncreaseDetailRepo salaryIncreaseDetailRepo,
            CurrentUser currentUser,
            EmailHelper emailHelper,
            //IEmailQueueService emailQueue,
            IOptions<SalaryIncreaseMailSettings> salaryIncreaseMailSettings,
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            ILogger<SalaryIncreaseController> logger)
        {
            _salaryIncreaseRepo = salaryIncreaseRepo;
            _salaryIncreaseDetailRepo = salaryIncreaseDetailRepo;
            _currentUser = currentUser;
            _emailHelper = emailHelper;
            //_emailQueue = emailQueue;
            _salaryIncreaseMailSettings = salaryIncreaseMailSettings.Value;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public class SalaryIncreaseSearchParam
        {
            public string? Keyword { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
        [RequiresPermission("N2")]
        [HttpPost("search-master")]
        public IActionResult SearchMaster([FromBody] SalaryIncreaseSearchParam param)
        {
            try
            {
                DateTime? sd = null;
                DateTime? ed = null;
                if (param.StartDate.HasValue)
                {
                    sd = param.StartDate.Value.ToLocalTime().Date;
                }
                if (param.EndDate.HasValue)
                {
                    ed = param.EndDate.Value.ToLocalTime().Date.AddDays(1).AddSeconds(-1);
                }

                string procedureName = "spGetSalaryIncreaseMaster";
                string[] paramNames = new string[] { "@Keyword", "@StartDate", "@EndDate" };
                object[] paramValues = new object[] {
                    param.Keyword ?? "",
                    (object)sd ?? DBNull.Value,
                    (object)ed ?? DBNull.Value
                };

                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var result = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(result, "Lấy danh sách đợt tăng lương thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N2")]

        [HttpPost("save-master")]
        public async Task<IActionResult> SaveMaster([FromBody] SalaryIncrease dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                }

                if (dto.ID <= 0)
                {
                    await _salaryIncreaseRepo.CreateAsync(dto);
                }
                else
                {
                    var entity = _salaryIncreaseRepo.GetByID(dto.ID);
                    if (entity != null)
                    {
                        entity.Code = dto.Code;
                        entity.Name = dto.Name;
                        entity.EffectiveDate = dto.EffectiveDate;
                        entity.MonthFrom = dto.MonthFrom;
                        entity.MonthTo = dto.MonthTo;
                        await _salaryIncreaseRepo.UpdateAsync(entity);
                    }
                }

                return Ok(ApiResponseFactory.Success(dto, "Lưu đợt tăng lương thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N2")]

        [HttpPost("delete-master")]
        public async Task<IActionResult> DeleteMaster([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bản ghi để xóa"));
                }

                foreach (var id in ids)
                {
                    var entity = await _salaryIncreaseRepo.GetByIDAsync(id);
                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                        await _salaryIncreaseRepo.UpdateAsync(entity);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa đợt tăng lương thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class SalaryIncreaseDetailParam
        {
            public int SalaryIncreaseID { get; set; }
            public string? Keyword { get; set; }
        }
        [RequiresPermission("N2")]

        [HttpPost("search-detail")]
        public IActionResult SearchDetail([FromBody] SalaryIncreaseDetailParam param)
        {
            try
            {
                string procedureName = "spGetSalaryIncreaseDetail";
                string[] paramNames = new string[] { "@SalaryIncreaseID", "@Keyword" };
                object[] paramValues = new object[] {
                    param.SalaryIncreaseID,
                    param.Keyword ?? ""
                };

                var data = SQLHelper<object>.ProcedureToList(procedureName, paramNames, paramValues);
                var result = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(result, "Lấy danh sách chi tiết nhân viên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N2")]

        [HttpPost("save-detail")]
        public async Task<IActionResult> SaveDetail([FromBody] SalaryIncreaseDetail dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                }

                var isDuplicate = _salaryIncreaseDetailRepo.GetAll(x =>
                    x.SalaryIncreaseID == dto.SalaryIncreaseID &&
                    x.EmployeeID == dto.EmployeeID &&
                    x.IsDeleted != true &&
                    x.ID != dto.ID).Any();
                if (isDuplicate)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Nhân viên này đã có trong đợt tăng lương, vui lòng chọn nhân viên khác"));
                }

                if (dto.ID <= 0)
                {
                    await _salaryIncreaseDetailRepo.CreateAsync(dto);
                }
                else
                {
                    var entity = _salaryIncreaseDetailRepo.GetByID(dto.ID);
                    if (entity != null)
                    {
                        entity.EmployeeID = dto.EmployeeID;
                        entity.EmailTBP = dto.EmailTBP;
                        entity.PreviousBaseSalary = dto.PreviousBaseSalary;
                        entity.CurrentBaseSalary = dto.CurrentBaseSalary;
                        entity.IsSend = dto.IsSend;
                        await _salaryIncreaseDetailRepo.UpdateAsync(entity);
                    }
                }

                return Ok(ApiResponseFactory.Success(dto, "Lưu chi tiết nhân viên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N2")]

        [HttpPost("delete-detail")]
        public async Task<IActionResult> DeleteDetail([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn bản ghi để xóa"));
                }

                foreach (var id in ids)
                {
                    var entity = await _salaryIncreaseDetailRepo.GetByIDAsync(id);
                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                        await _salaryIncreaseDetailRepo.UpdateAsync(entity);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Xóa chi tiết nhân viên thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N2")]

        [HttpPost("save-data-detail")]
        public async Task<IActionResult> SaveDataDetail([FromBody] List<SalaryIncreaseDetail> items)
        {
            if (items == null || items.Count == 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Không có dữ liệu để lưu"));
            }

            var successCount = 0;
            var skippedCount = 0;
            var errors = new List<string>();

            // Nhân viên đã có sẵn trong đợt (không tính các dòng đang tự cập nhật lại - ID > 0)
            var salaryIncreaseIDs = items.Where(x => x.SalaryIncreaseID > 0).Select(x => x.SalaryIncreaseID!.Value).Distinct().ToList();
            var existingEmployeeIds = salaryIncreaseIDs.Count > 0
                ? _salaryIncreaseDetailRepo.GetAll(x => salaryIncreaseIDs.Contains(x.SalaryIncreaseID ?? 0) && x.IsDeleted != true)
                    .Select(x => x.EmployeeID ?? 0).ToHashSet()
                : new HashSet<int>();
            var seenInBatch = new HashSet<int>();

            foreach (var item in items)
            {
                try
                {
                    if (item.EmployeeID == null || item.EmployeeID <= 0)
                    {
                        throw new Exception("Thiếu mã nhân viên");
                    }
                    if (item.SalaryIncreaseID == null || item.SalaryIncreaseID <= 0)
                    {
                        throw new Exception("Thiếu đợt tăng lương");
                    }

                    // Chỉ bỏ qua trùng với dòng mới thêm (ID <= 0); dòng đang update lại chính nó thì vẫn cho qua.
                    if (item.ID <= 0 && (existingEmployeeIds.Contains(item.EmployeeID.Value) || !seenInBatch.Add(item.EmployeeID.Value)))
                    {
                        skippedCount++;
                        continue;
                    }

                    if (item.ID > 0)
                    {
                        var entity = _salaryIncreaseDetailRepo.GetByID(item.ID);
                        if (entity != null)
                        {
                            entity.EmployeeID = item.EmployeeID;
                            entity.EmailTBP = item.EmailTBP;
                            entity.PreviousBaseSalary = item.PreviousBaseSalary;
                            entity.CurrentBaseSalary = item.CurrentBaseSalary;
                            await _salaryIncreaseDetailRepo.UpdateAsync(entity);
                        }
                    }
                    else
                    {
                        await _salaryIncreaseDetailRepo.CreateAsync(item);
                    }

                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"NV {item.EmployeeID}: {ex.Message}");
                }
            }

            var parts = new List<string> { $"Nhập thành công {successCount}/{items.Count} nhân viên" };
            if (skippedCount > 0) parts.Add($"bỏ qua {skippedCount} nhân viên đã có trong đợt");
            if (errors.Count > 0) parts.Add($"lỗi: {string.Join("; ", errors)}");
            var message = string.Join(", ", parts);

            return Ok(ApiResponseFactory.Success(null, message));
        }

        [HttpGet("mail-config")]
        public IActionResult GetMailConfig()
        {
            var result = new
            {
                _salaryIncreaseMailSettings.BGDEmail,
                _salaryIncreaseMailSettings.HRMEmail,
                _salaryIncreaseMailSettings.KTTEmail,
                _salaryIncreaseMailSettings.TestRecipientEmail,
                // Footer công ty được gắn vào mail lúc gửi thật (xem SendMail/SendMailQueue) -
                // trả về đây để frontend ghép vào khi xem trước cho đúng với mail thật.
                Footer = _configuration["FooterMail:HR:Footer"] ?? ""
            };
            return Ok(ApiResponseFactory.Success(result, "Lấy cấu hình email thành công"));
        }
        [RequiresPermission("N2")]

        [HttpPost("send-mail")]
        public async Task<IActionResult> SendMail([FromBody] List<SalaryIncreaseSendMailParam> items)
        {
            if (items == null || items.Count == 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn nhân viên cần gửi mail"));
            }

            var results = new List<SalaryIncreaseSendMailResultItem>();

            foreach (var item in items)
            {
                var result = new SalaryIncreaseSendMailResultItem { DetailID = item.DetailID };
                try
                {
                    // Giai đoạn test: ép toàn bộ mail về TestRecipientEmail thay vì email thật của nhân viên.
                    var emailTo = !string.IsNullOrWhiteSpace(_salaryIncreaseMailSettings.TestRecipientEmail)
                        ? _salaryIncreaseMailSettings.TestRecipientEmail
                        : item.EmailTo;

                    if (string.IsNullOrWhiteSpace(emailTo))
                    {
                        throw new Exception("Nhân viên chưa có email công ty");
                    }

                    await _emailHelper.SendAsync(emailTo, item.Subject, item.Body, true, item.EmailCC);

                    var entity = await _salaryIncreaseDetailRepo.GetByIDAsync(item.DetailID);
                    if (entity != null)
                    {
                        entity.IsSend = true;
                        await _salaryIncreaseDetailRepo.UpdateAsync(entity);
                    }

                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.ErrorMessage = ex.Message;
                    _logger.LogError(
                        "Gửi mail điều chỉnh lương thất bại: DetailID={DetailID}, Email={Email}, Lỗi={Error}",
                        item.DetailID, item.EmailTo, ex.Message);
                }

                results.Add(result);
            }

            var failCount = results.Count(x => !x.Success);
            var message = failCount == 0
                ? $"Gửi mail thành công cho {results.Count} nhân viên"
                : $"Gửi thành công {results.Count - failCount}/{results.Count} nhân viên, {failCount} nhân viên gửi thất bại";

            return Ok(ApiResponseFactory.Success(results, message));
        }

        /// <summary>
        /// Bản dùng hàng đợi nền (EmailQueueService): trả về ngay sau khi đưa vào hàng đợi,
        /// không chờ gửi SMTP thật xong. Phù hợp khi chọn gửi số lượng lớn nhân viên cùng lúc.
        /// IsSend được đánh dấu ngay (best-effort) - nếu gửi thất bại ở nền, lỗi được ghi log
        /// phía server chứ không trả về ở response này.
        /// </summary>
        //[HttpPost("send-mail-queue")]
        //public async Task<IActionResult> SendMailQueue([FromBody] List<SalaryIncreaseSendMailParam> items)
        //{
        //    if (items == null || items.Count == 0)
        //    {
        //        return Task.FromResult<IActionResult>(BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn nhân viên cần gửi mail")));
        //    }

        //    var footer = _configuration["FooterMail:HR:Footer"] ?? "";

        //    foreach (var item in items)
        //    {
        //        var emailTo = !string.IsNullOrWhiteSpace(_salaryIncreaseMailSettings.TestRecipientEmail)
        //            ? _salaryIncreaseMailSettings.TestRecipientEmail
        //            : item.EmailTo;

        //        if (string.IsNullOrWhiteSpace(emailTo))
        //        {
        //            _logger.LogWarning("Bỏ qua gửi mail lương: DetailID={DetailID} chưa có email công ty", item.DetailID);
        //            continue;
        //        }

        //        var detailId = item.DetailID;

        //        _emailQueue.Enqueue(new EmailQueueMessage
        //        {
        //            Profile = MailProfile.Default,
        //            ToEmail = emailTo,
        //            Cc = item.EmailCC,
        //            Subject = item.Subject,
        //            Body = item.Body + footer,
        //            // Callback chạy ở nền sau khi thử gửi xong - tự tạo scope DI riêng vì
        //            // scope của request HTTP gốc đã bị dispose từ lâu vào lúc này chạy.
        //            OnResult = async (success, error) =>
        //            {
        //                using var scope = _scopeFactory.CreateScope();
        //                var repo = scope.ServiceProvider.GetRequiredService<SalaryIncreaseDetailRepo>();
        //                var entity = await repo.GetByIDAsync(detailId);
        //                if (entity != null)
        //                {
        //                    entity.IsSend = success;
        //                    await repo.UpdateAsync(entity);
        //                }

        //                if (!success)
        //                {
        //                    _logger.LogError(
        //                        "Gửi mail điều chỉnh lương thất bại: DetailID={DetailID}, Email={Email}, Lỗi={Error}",
        //                        detailId, emailTo, error);
        //                }
        //            }
        //        });
        //    }

        //    return Task.FromResult<IActionResult>(Ok(ApiResponseFactory.Success(null, $"Đã đưa {items.Count} mail vào hàng đợi gửi")));
        //}
    }
}
