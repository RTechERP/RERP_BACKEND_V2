using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.HRM;

namespace RERPAPI.Controllers.HRM.Employees
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeNightShiftController : ControllerBase
    {
        vUserGroupLinksRepo _vUserGroupLinksRepo;
        EmployeeNightShiftRepo _employeeNightShiftRepo;
        public EmployeeNightShiftController(vUserGroupLinksRepo vUserGroupLinksRepo, EmployeeNightShiftRepo employeeNightShiftRepo)
        {
            _vUserGroupLinksRepo = vUserGroupLinksRepo;
            _employeeNightShiftRepo = employeeNightShiftRepo;

        }
        [RequiresPermission("N1,N2")]
        [HttpPost("get-employee-night-shift")]
        public ActionResult GetEmployeeNightShift([FromBody] EmployeeNightShiftRequestParam request)

        {
            try
            {
                var data = SQLHelper<dynamic>.ProcedureToList(
                    "spGetEmployeeNightShift",
                    new string[] { "@EmployeeID", "@DateStart", "@DateEnd", "@IsApproved", "@DepartmentID", "@Keyword", "@PageNumber", "PageSize" },
                    new object[] { 0, request.DateStart, request.DateEnd, request.IsApproved, request.DepartmentID ?? 0, request.KeyWord ?? "", request.Page ?? 1, request.Size ?? 50 });

                var nightShiftdata = SQLHelper<object>.GetListData(data, 0);
                var TotalPage = SQLHelper<object>.GetListData(data, 2);
                return Ok(ApiResponseFactory.Success(new { nightShiftdata, TotalPage }, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("get-employee-night-shift-summary")]
        public ActionResult GetEmployeeNightShiftSummary([FromBody] EmployeeNightShiftSummaryRequestParam request)

        {
            try
            {
                var data = SQLHelper<dynamic>.ProcedureToList
                ("spGetEmployeeNightShiftByMonth"
                , new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID", "@Keyword" }
                , new object[] { request.Month, request.Year, request.DepartmentID, request.EmployeeID, request.KeyWord ?? "" });

                var nightShiftdata = SQLHelper<object>.GetListData(data, 0);

                return Ok(ApiResponseFactory.Success(nightShiftdata, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] List<EmployeeNighShift> nightShifts)
        {
            try
            {
                if (nightShifts == null) { return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." }); }
                if (nightShifts != null && nightShifts.Any())
                {
                    foreach (var item in nightShifts)
                    {
                        if (!_employeeNightShiftRepo.Validate(item, out string message))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, message));
                        }

                        if (item.ID <= 0)
                            await _employeeNightShiftRepo.CreateAsync(item);
                        else
                            await _employeeNightShiftRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(nightShifts, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        [RequiresPermission("N1,N2")]
        [HttpPost("save-approve-hr")]
        public async Task<IActionResult> SaveApproveHr([FromBody] List<EmployeeNighShift> nightShifts)
        {
            try
            {
                if (nightShifts == null) { return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." }); }
                if (nightShifts != null && nightShifts.Any())
                {
                    foreach (var item in nightShifts)
                    {

                        if (item.ID <= 0)
                            await _employeeNightShiftRepo.CreateAsync(item);
                        else
                            await _employeeNightShiftRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(nightShifts, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
        [RequiresPermission("N1,N2")]
        [HttpPost("save-approve-tbp")]
        public async Task<IActionResult> SaveApproveTBP([FromBody] List<EmployeeNighShift> nightShifts)
        {
            try
            {
                if (nightShifts == null) { return BadRequest(new { status = 0, message = "Dữ liệu gửi lên không hợp lệ." }); }
                if (nightShifts != null && nightShifts.Any())
                {
                    foreach (var item in nightShifts)
                    {

                        if (item.ID <= 0)
                            await _employeeNightShiftRepo.CreateAsync(item);
                        else
                            await _employeeNightShiftRepo.UpdateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(nightShifts, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }
    }
}
