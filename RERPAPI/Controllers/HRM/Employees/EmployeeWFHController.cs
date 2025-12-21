using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Model.Param.HRM;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;
using System.Data;
namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeWFHController : ControllerBase
    {
        private readonly EmployeeWFHRepo _employeeWFHRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly vUserGroupLinksRepo _vUserGroupLinksRepo;
        EmployeeRepo _employeeRepo;
        EmployeeSendEmailRepo _employeeSendEmailRepo;

        public EmployeeWFHController(EmployeeWFHRepo employeeWFHRepo, DepartmentRepo departmentRepo, vUserGroupLinksRepo vUserGroupLinksRepo, EmployeeRepo employeeRepo, EmployeeSendEmailRepo employeeSendEmailRepo)
        {
            _employeeWFHRepo = employeeWFHRepo;
            _departmentRepo = departmentRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _employeeRepo = employeeRepo;
            _employeeSendEmailRepo = employeeSendEmailRepo;
        }

        //[RequiresPermission("N1,N2")]
        [HttpPost("get-wfh")]
        public IActionResult GetWFH([FromBody] EmployeeWFHRequestParam  request)
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
                    employeeID = request.EmployeeId;
                }
                else
                {
                    employeeID = currentUser.EmployeeID;
                }
                // Gọi stored procedure
                var dt = SQLHelper<EmployeeWFHDTO>.ProcedureToList("spGetWFH",
                    new string[] { "@PageNumber", "@PageSize", "@Year", "@Month", "@Keyword", "@DepartmentID", "@EmployeeID", "@IDApprovedTP", "@Status" },
                    new object[] { request.Page, request.Size, request.Year, request.Month, request.Keyword, request.DepartmentId, employeeID, request.IdApprovedTP, request.Status });
                // Dữ liệu trang hiện tại
                var data = SQLHelper<EmployeeWFHDTO>.GetListData(dt, 0);
                // Tổng số trang
                var totalPage = SQLHelper<object>.GetListData(dt, 1);
                return Ok(ApiResponseFactory.Success(new { data, totalPage }, "Lấy dữ lệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-wfh-person")]
        public IActionResult GetWFHPerson([FromBody] EmployeeOnLeavePersonParam request)
        {
            try
            {
                var employeelate = SQLHelper<object>.ProcedureToList("spGetWFH_New", new string[] { "@PageNumber", "@PageSize", "@Keyword", "@StartDate", "@EndDate", "@IDApprovedTP", "@Status", "@DepartmentID" },
               new object[] { request.Page, request.Size, request.Keyword ?? "", request.DateStart, request.DateEnd, request.IDApprovedTP, request.Status, request.DepartmentID });

                var data = SQLHelper<object>.GetListData(employeelate, 0);
                var TotalPages = SQLHelper<object>.GetListData(employeelate, 1);
                return Ok(ApiResponseFactory.Success(new { data, TotalPages }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("list-summary-employee-work-form-home")]

        public IActionResult ListSummaryEmployeeOnleavePerson(EmployeeWorkFormHomeSummaryParam request)
        {
            try
            {
                var employeeWFHSummary = SQLHelper<object>.ProcedureToList("spGetEmployeeOnLeaveInWeb", new string[] { "@Keyword", "@DateStart", "@DateEnd", "@IsApproved", "@TimeWFH", "@DepartmentID", "@EmployeeID" },
               new object[] { request.Keyword ?? "", request.DateStart, request.DateEnd, request.IsApproved, request.TimeWFH, request.DepartmentID ?? 0, request.EmployeeID ?? 0 });

                var data = SQLHelper<object>.GetListData(employeeWFHSummary, 0);
                var TotalPages = SQLHelper<object>.GetListData(employeeWFHSummary, 1);
                return Ok(ApiResponseFactory.Success(new { data, TotalPages }, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[RequiresPermission("N1,N2")]
        [HttpGet("wfh-detail/{id}")]
        public IActionResult GetWFHDetail(int id)
        {
            try
            {
                var wfhDetail = _employeeWFHRepo.GetByID(id);
                if (wfhDetail == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy thông tin WFH"));
                }

                return Ok(ApiResponseFactory.Success(wfhDetail, "Lấy chi tiết WFH thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //[RequiresPermission("N1,N2")]
        [HttpGet("get-department")]
        public IActionResult GetDepartment()
        {
            try
            {
                var departments = _departmentRepo.GetAll()
                    .Select(x => new { x.ID, x.Code, x.Name })
                    .ToList();

                return Ok(new { status = 1, data = departments });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //[RequiresPermission("N1,N2")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] EmployeeWFH employeeWFH)
        {
            try
            {

                if (!_employeeWFHRepo.Validate(employeeWFH, out string message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (employeeWFH.ID <= 0)
                {
                  var result=  await _employeeWFHRepo.CreateAsync(employeeWFH);
                    if(result>0)
                    {
                        var employee = _employeeRepo.GetByID(employeeWFH.EmployeeID ?? 0);
                        var employeeTP = _employeeRepo.GetByID(employeeWFH.ApprovedID ?? 0);
                        string type = employeeWFH.TimeWFH == 1 ? " buổi sáng" : (employeeWFH.TimeWFH == 2 ? " buổi chiều" : "");

                        string subject = $"WFH{type.ToUpper()} - {employee.FullName??"".ToUpper()} - {employeeWFH.DateWFH.Value.ToString("dd/MM/yyyy")}";
                        string body = "<div> <p style=\"font - weight: bold; color: red;\">[NO REPLY]</p> <p>Dear anh/chị " + employeeTP.FullName + "</p> </div> <div style=\"margin-top: 30px;\"> " +
                                      $"<p>Em xin phép anh/chị cho em làm việc tại nhà{type} ngày {employeeWFH.DateWFH.Value.ToString("dd/MM/yyyy")}.</p> " +
                                      "<p>Lý do: " + employeeWFH.Reason + "</p> " +
                                      "<p>Anh/chị duyệt giúp em với ạ. Em cảm ơn!</p> </div>" +
                                      "<div style=\"margin-top: 30px;\"> <p>Thanks</p> <p>" + employee.FullName + "</p> </div>";

                        _employeeSendEmailRepo.SendMail(employeeWFH.EmployeeID ?? 0, employeeWFH.ApprovedID ?? 0, subject, body, "");
                    }    
                }
                else
                {
                    await _employeeWFHRepo.UpdateAsync(employeeWFH);
                }
                return Ok(ApiResponseFactory.Success(employeeWFH, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("save-approve-hr")]
        public async Task<IActionResult> SaveApproveHR([FromBody] EmployeeWFH employeeWFH)
        {
            try
            {

             
                if (employeeWFH.ID <= 0) await _employeeWFHRepo.CreateAsync(employeeWFH);
                else await _employeeWFHRepo.UpdateAsync(employeeWFH);
                return Ok(ApiResponseFactory.Success(employeeWFH, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-approve-tbp")]
        public async Task<IActionResult> SaveApproveTBP([FromBody] EmployeeWFH employeeWFH)
            {
            try
            {


                if (employeeWFH.ID <= 0) await _employeeWFHRepo.CreateAsync(employeeWFH);
                else await _employeeWFHRepo.UpdateAsync(employeeWFH);
                return Ok(ApiResponseFactory.Success(employeeWFH, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employee-approver")]
        public IActionResult GetEmployeesWithApprovers()
        {
            try
            {
                // Gọi store procedure
                var dataSet = SQLHelper<Object>.ProcedureToList("spGetEmployeeAndEmployeeApprover", new string[] { }, new object[] { });

                var employees = SQLHelper<object>.GetListData(dataSet, 0)
                                     .Select(x => new
                                     {
                                         x.ID,
                                         x.FullName,
                                         x.DepartmentName,
                                         x.Code

                                     }).ToList();

                var approvers = SQLHelper<object>.GetListData(dataSet, 1)
                                                         .Select(x => new
                                                         {
                                                             x.EmployeeID,
                                                             x.FullName,
                                                             x.DepartmentName,
                                                             x.Code

                                                         }).ToList();

                // Trả về kết quả
                return Ok(new
                {
                    status = 1,
                    message = "Lấy danh sách nhân viên và người phê duyệt thành công",
                    data = new
                    {
                        employees,
                        approvers
                    },
                });
            }
            catch (Exception ex)

            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("check-duplicate-wfh/{id}/{employeeId}/{date}/{timeWFH}")]
        public IActionResult CheckDuplicateWFH(int id, int employeeId, string date, int timeWFH)
        {
            try
            {
                bool isDuplicate = false;

                // Chuyển đổi chuỗi ngày sang DateTime
                DateTime dateWFH;
                if (!DateTime.TryParse(date, out dateWFH))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Định dạng ngày không hợp lệ!"
                    });
                }

                var existWFH = _employeeWFHRepo.GetAll()
                    .Where(x => x.ID != id &&
                                x.EmployeeID == employeeId &&
                                x.DateWFH.HasValue &&
                                x.DateWFH.Value.Date == dateWFH.Date &&
                                x.TimeWFH == timeWFH
                                //&& x.IsDelete == false
                                );

                if (existWFH.Any())
                {
                    isDuplicate = true;
                }

                return Ok(new
                {
                    status = 1,
                    data = isDuplicate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


    }
}