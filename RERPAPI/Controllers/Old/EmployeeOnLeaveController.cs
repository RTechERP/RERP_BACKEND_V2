using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using RERPAPI.Model.DTO.HRM;
using Microsoft.Extensions.Configuration;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeOnLeaveController : ControllerBase
    {
        private readonly EmployeeOnLeaveRepo _employeeOnLeaveRepo;
        private readonly EmployeeRepo _employeeRepo;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;
        private readonly EmployeeSendEmailRepo _employeeSendEmailRepo;
        private readonly EmployeeOnLeavePhaseRepo _employeeOnLeavePhaseRepo;
        private readonly EmailHelper _emailHelper;
        private readonly IConfiguration _configuration;

        public EmployeeOnLeaveController(
            EmployeeOnLeaveRepo employeeOnLeaveRepo,
            EmployeeRepo employeeRepo,
            vUserGroupLinksRepo vUserGroupLinksRepo,
            EmployeeSendEmailRepo employeeSendEmailRepo,
            EmployeeOnLeavePhaseRepo employeeOnLeavePhaseRepo,
            EmailHelper emailHelper,
            IConfiguration configuration)
        {
            _employeeOnLeaveRepo = employeeOnLeaveRepo;
            _employeeRepo = employeeRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _employeeSendEmailRepo = employeeSendEmailRepo;
            _employeeOnLeavePhaseRepo = employeeOnLeavePhaseRepo;
            _emailHelper = emailHelper;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult GetAllEmployeeOnLeave(EmployeeOnLeaveParam param)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var vUserHR = _vUserGroupLinksRepo
                              .GetAll()
                              .FirstOrDefault(x =>
                               (x.Code == "N1" || x.Code == "N2") &&
                               x.UserID == currentUser.ID);

                int employeeID;
                if (vUserHR != null)
                {
                    employeeID = param.employeeId;
                }
                else
                {
                    employeeID = currentUser.EmployeeID;
                }
                var employeeOnLeaves = SQLHelper<object>.ProcedureToList("spGetDayOff",
                    new string[] { "@PageNumber", "@PageSize", "@Keyword", "@Month", "@Year", "@IDApprovedTP", "@Status", "@DepartmentID", "@EmployeeID" },
                    new object[] { param.pageNumber, param.pageSize, param.keyWord, param.month, param.year, param.IDApprovedTP, param.status, param.departmentId, employeeID });

                var data = SQLHelper<object>.GetListData(employeeOnLeaves, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("get-employee-onleave-person")]
        public IActionResult GetEmployeeOnLeavePerson(EmployeeOnLeavePersonParam request)
        {
            try
            {
                var employeeOnLeaves = SQLHelper<object>.ProcedureToList("spGetDayOff_New", new string[] { "@PageNumber", "@PageSize", "@Keyword", "@DateStart", "@DateEnd", "@IDApprovedTP", "@Status", "@DepartmentID" },
               new object[] { request.Page, request.Size, request.Keyword ?? "", request.DateStart, request.DateEnd, request.IDApprovedTP, request.Status, request.DepartmentID ?? 0 });

                var data = SQLHelper<object>.GetListData(employeeOnLeaves, 0);
                var TotalPages = SQLHelper<object>.GetListData(employeeOnLeaves, 1);
                return Ok(ApiResponseFactory.Success(new { data, TotalPages }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("list-summary-employee-on-leave")]
        public IActionResult ListSummaryEmployeeOnleave(EmployeeOnleaveSummaryParam request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                var vUserHR = _vUserGroupLinksRepo
                              .GetAll()
                              .FirstOrDefault(x =>
                               (x.Code == "N1" || x.Code == "N2") &&
                               x.UserID == currentUser.ID);

                int employeeID;
                if (vUserHR != null)
                {
                    employeeID = request.EmployeeID ?? 0;
                }
                else
                {
                    employeeID = currentUser.EmployeeID;
                }
                var employeeOnLeaveSummary = SQLHelper<object>.ProcedureToList("spGetEmployeeOnLeaveInWeb", new string[] { "@DateStart", "@EmployeeID" },
               new object[] { request.DateStart, employeeID });

                var data = SQLHelper<object>.GetListData(employeeOnLeaveSummary, 1);
                var TotalPages = SQLHelper<object>.GetListData(employeeOnLeaveSummary, 1);
                return Ok(ApiResponseFactory.Success(new { data, TotalPages }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("list-summary-employee-on-leave-person")]
        public IActionResult ListSummaryEmployeeOnleavePerson(EmployeeOnleaveSummaryParam request)
        {
            try
            {
                var employeeOnLeaveSummary = SQLHelper<object>.ProcedureToList("spGetEmployeeOnLeaveInWeb", new string[] { "@Keyword", "@DateStart", "@DateEnd", "@IsApproved", "@Type", "@DepartmentID", "@EmployeeID" },
               new object[] { request.Keyword ?? "", request.DateStart, request.DateEnd, request.IsApproved, request.Type, request.DepartmentID ?? 0, request.EmployeeID ?? 0 });

                var data = SQLHelper<object>.GetListData(employeeOnLeaveSummary, 0);
                var TotalPages = SQLHelper<object>.GetListData(employeeOnLeaveSummary, 1);
                return Ok(ApiResponseFactory.Success(new { data, TotalPages }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet]
        public IActionResult GetSummaryEmployeeOnLeave(int month, int year, string? keyWord)
        {
            try
            {
                var summary = SQLHelper<object>.ProcedureToList("spGetEmployeeOnleaveByMonth", new string[] { "@Month", "@Year", "@KeyWord" },
                                       new object[] { month, year, keyWord ?? "" });
                var data = SQLHelper<object>.GetListData(summary, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveEmployeeOnLeave(EmployeeOnLeave employeeOnLeave)
        {
            try
            {
                if (employeeOnLeave.StartDate.HasValue)
                {
                    employeeOnLeave.StartDate = DateTime.SpecifyKind(employeeOnLeave.StartDate.Value, DateTimeKind.Utc);
                }
                if (employeeOnLeave.EndDate.HasValue)
                {
                    employeeOnLeave.EndDate = DateTime.SpecifyKind(employeeOnLeave.EndDate.Value, DateTimeKind.Utc);
                }

                var existingLeaves = _employeeOnLeaveRepo.GetAll()
                    .Where(x => x.EmployeeID == employeeOnLeave.EmployeeID
                        && x.ID != employeeOnLeave.ID
                        && x.StartDate.HasValue
                        && employeeOnLeave.StartDate.HasValue
                        && x.StartDate.Value.Date == employeeOnLeave.StartDate.Value.Date
                        && x.DeleteFlag != true
                        && (
                            // Nếu bản ghi existing là nghỉ cả ngày (TimeOnLeave == 3)
                            x.TimeOnLeave == 3
                            // Hoặc bản ghi mới đăng ký là nghỉ cả ngày
                            || employeeOnLeave.TimeOnLeave == 3
                            // Hoặc cùng buổi (sáng-sáng hoặc chiều-chiều)
                            || x.TimeOnLeave == employeeOnLeave.TimeOnLeave
                        )
                        && employeeOnLeave.DeleteFlag != true);

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
                var vUserHR = _vUserGroupLinksRepo
                           .GetAll()
                           .FirstOrDefault(x =>
                            (x.Code == "N1" || x.Code == "N2") &&
                            x.UserID == currentUser.ID);

                if (employeeOnLeave.ID > 0 && vUserHR == null && currentUser.EmployeeID != employeeOnLeave.EmployeeID)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể sửa phiếu của người khác"));
                }
                if (existingLeaves.Any())
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Nhân viên đã đăng ký nghỉ ngày "
                        + employeeOnLeave.StartDate.Value.ToString("dd/MM/yyyy")
                        + ". Vui lòng chọn ngày khác."));
                }

                if (employeeOnLeave.ID <= 0)
                {
                    var result = await _employeeOnLeaveRepo.CreateAsync(employeeOnLeave);
                    if (result > 0)
                    {
                        var employee = _employeeRepo.GetByID(employeeOnLeave.EmployeeID ?? 0);
                        var employeeTP = _employeeRepo.GetByID(employeeOnLeave.ApprovedTP ?? 0);
                        string timeonleave = employeeOnLeave.TimeOnLeave == 1 ? "buổi sáng ngày" : employeeOnLeave.TimeOnLeave == 2 ? "buổi chiều ngày" : "ngày";

                        //Loại nghỉ
                        string type = employeeOnLeave.TypeIsReal == 1 ? "nghỉ không lương" : (employeeOnLeave.TypeIsReal == 2 ? "nghỉ phép" : "nghỉ việc riêng có hưởng lương");
                        string subject = $"NGHỈ - {employee.FullName ?? "".ToUpper()} - {employeeOnLeave.StartDate.Value.ToString("dd/MM/yyyy")}";
                        string body = "<div> <p style=\"font - weight: bold; color: red;\">[NO REPLY]</p> <p>Dear anh/chị " + employeeTP.FullName + "</p> </div> <div style=\"margin-top: 30px;\"> " +
                                      $"<p>Em xin phép anh/chị cho em {type} {timeonleave} {employeeOnLeave.StartDate.Value.ToString("dd/MM/yyyy")}.</p> " +
                                      "<p>Lý do: " + employeeOnLeave.Reason + "</p> " +
                                      "<p>Anh/chị duyệt giúp em với ạ. Em cảm ơn!</p> </div>" +
                                      "<div style=\"margin-top: 30px;\"> <p>Thanks</p> <p>" + employee.FullName + "</p> </div>";
                        //_employeeSendEmailRepo.SendMail(employeeOnLeave.EmployeeID ?? 0, employeeOnLeave.ApprovedTP ?? 0, subject, body, "");

                        string cc = string.IsNullOrEmpty(employee.EmailCongTy) ? (employee.EmailCaNhan ?? "") : employee.EmailCongTy;
                        _emailHelper.SendAsync(employeeTP.EmailCongTy ?? "", subject, body, true, cc);
                    }
                }
                else
                {
                    await _employeeOnLeaveRepo.UpdateAsync(employeeOnLeave);
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpPost("GetEmployeeOnLeavePerson")]
        //public async Task<IActionResult> GetEmployeeOnLeavePerson(EmployeeOnLeaveParam param)
        //{
        //    try
        //    {
        //        var result = await _employeeOnLeaveRepo.GetEmployeeOnLeavePerson(param);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("SaveMultiPhase")]
        public async Task<IActionResult> SaveMultiPhase(EmployeeOnLeaveMultiDTO dto)
        {
            try
            {
                var validate = _employeeOnLeavePhaseRepo.Validate(dto);
                if (validate.status == 0)
                {
                    return BadRequest(validate);
                }

                var result = await _employeeOnLeavePhaseRepo.SaveMultiPhase(dto);

                // Gửi email sau khi lưu thành công (pattern giống HRRecruitmentCandidateController)
                if (dto.Details != null && dto.Details.Any())
                {
                    var firstDetail = dto.Details.First();
                    var employee = _employeeRepo.GetByID(dto.Phase.EmployeeID ?? 0);
                    var employeeTP = _employeeRepo.GetByID(firstDetail.ApprovedTP ?? 0);

                    if (employee != null && employeeTP != null)
                    {
                        //string mailTo = employeeTP.EmailCongTy ?? employeeTP.EmailCom
                        //                ?? employeeTP.EmailCaNhan ?? employeeTP.Email ?? "";
                        string mailTo = "rtcmodula@gmail.com";

                        if (!string.IsNullOrWhiteSpace(mailTo))
                        {
                            string subject = $"ĐĂNG KÝ NGHỈ - {(employee.FullName ?? "").ToUpper()} -{dto.Phase.DateRegister:dd/MM/yyyy}";
                            string approveUrl = "https://erp.rtc.edu.vn/rerpweb/tbp-approve";
                            string cards = "";

                            foreach (var detail in dto.Details)
                            {
                                string time = detail.TimeOnLeave == 1 ? "Sáng"
                                             : detail.TimeOnLeave == 2 ? "Chiều"
                                             : "Cả ngày";

                                string type = detail.TypeIsReal == 1 ? "Nghỉ không lương"
                                             : detail.TypeIsReal == 2 ? "Nghỉ phép"
                                             : "Nghỉ việc riêng có hưởng lương";

                                string date = detail.StartDate?.ToString("dd/MM/yyyy") ?? "";

                                string dateDisplay = time != "Cả ngày"
                                    ? $"{date} ({time})"
                                    : date;

                                cards += $@"
                                    <div style='padding:10px 0;'>
                                        <p style='margin:2px 0'><b>Ngày:</b> {dateDisplay}</p>
                                        <p style='margin:2px 0'><b>Loại nghỉ:</b> {type}</p>
                                        <p style='margin:2px 0'><b>Lý do:</b> {detail.Reason}</p>
                                    </div>
                                    <div style='border-top:1px solid #eee;'></div>";}

                                string body = $@"
                                    <div>
                                        <p style='font-weight:bold;color:red;'>[NO REPLY]</p>
                                            <p>Dear anh/chị {employeeTP.FullName},</p>
                                        </div>

                                        <div style='margin-top:20px;'>
                                            <p>Em xin phép anh/chị cho em đăng ký nghỉ các ngày sau:</p>
                                            {cards}
                                            <p style='margin-top:20px;'>Anh/chị duyệt giúp em với ạ. Em cảm ơn!</p>
                                        </div>
                                  <p>
                                    Bấm vào 
                                    <a href='{approveUrl}' 
                                       style='
                                           color:#1890ff;
                                           font-weight:bold;
                                           text-decoration:underline;
                                       '>
                                       đây
                                    </a> 
                                    để duyệt
                                </p>
                                        <div style='margin-top:20px;'>
                                            <p>Thanks,</p>
                                            <p style='font-weight:bold;'>{employee.FullName}</p>
                                        </div>
                                  <p>
                                    </p>";
                            var footer = _configuration["FooterMail:HR:Footer"] ?? "";

                            await _emailHelper.SendAsync(
                                mailTo,
                                subject,
                                body + footer,
                                cc: ""
                            );
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(result, "Lưu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-multi/{id}")]
        public async Task<IActionResult> GetMultiByID(int id)
        {
            try
            {
                var result = await _employeeOnLeavePhaseRepo.GetMultiByID(id);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
