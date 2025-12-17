using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

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

        public EmployeeOnLeaveController(EmployeeOnLeaveRepo employeeOnLeaveRepo, EmployeeRepo employeeRepo, vUserGroupLinksRepo vUserGroupLinksRepo)
        {
            _employeeOnLeaveRepo = employeeOnLeaveRepo;
            _employeeRepo = employeeRepo;
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
        }

        [HttpPost]
        //[RequiresPermission("N2,N1")]
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
               new object[] {request.Page , request.Size,request.Keyword??"", request.DateStart, request.DateEnd, request.IDApprovedTP, request.Status, request.DepartmentID ?? 0 });

                var data = SQLHelper<object>.GetListData(employeeOnLeaves, 0);
                var TotalPages = SQLHelper<object>.GetListData(employeeOnLeaves, 1);
                return Ok(ApiResponseFactory.Success(new {data, TotalPages}, ""));
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
        //[RequiresPermission("N2,N1")]
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
        //[RequiresPermission("N2,N1")]
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
                        && x.TypeIsReal == employeeOnLeave.TypeIsReal
                        && x.ID != employeeOnLeave.ID
                        && x.StartDate.HasValue
                        && employeeOnLeave.StartDate.HasValue
                        && x.StartDate.Value.Date == employeeOnLeave.StartDate.Value.Date 
                        && x.DeleteFlag != true);

                if (existingLeaves.Any())
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Nhân viên đã đăng ký nghỉ ngày " 
                        + employeeOnLeave.StartDate.Value.ToString("dd/MM/yyyy") 
                        + ". Vui lòng chọn ngày khác."));
                }

                if (employeeOnLeave.ID <= 0)
                {
                    await _employeeOnLeaveRepo.CreateAsync(employeeOnLeave);
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
    }
}
