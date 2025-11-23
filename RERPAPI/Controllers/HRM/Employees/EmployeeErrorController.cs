using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.HRM;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.HRM.Employees
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeErrorController : ControllerBase
    {
        private readonly EmployeeErrorRepo _employeeErrorRepo;
        private readonly EmployeeRepo _employeeRepo;
        public EmployeeErrorController(EmployeeErrorRepo employeeErrorRepo, EmployeeRepo employeeRepo)
        {
            _employeeErrorRepo = employeeErrorRepo;
            _employeeRepo = employeeRepo;

        }
        [RequiresPermission("N1,N2")]
        [HttpPost("get-employee-error")]
        public IActionResult getEmployeeError([FromBody]  EmployeeErrorRequestParam request)
        {
            try
            {
                DateTime ds = new DateTime(request.DateStart.Year, request.DateStart.Month, request.DateStart.Day, 0, 0, 0);
                DateTime de = new DateTime(request.DateEnd.Year, request.DateEnd.Month, request.DateEnd.Day, 23, 59, 59);
                var dt = SQLHelper<object>.ProcedureToList("spLoadEmployeeError",
                                                new string[] { @"FilterText", "@PageNumber", "@PageSize", "@DateStart", "@DateEnd", },
                                                new object[] { request.KeyWord ?? "", request.Page??1, request.Size??50, ds, de });
                var data = SQLHelper<object>.GetListData(dt, 0);
                var totalPage = SQLHelper<object>.GetListData(dt, 1);
                return Ok(ApiResponseFactory.Success(new { data, totalPage }, "Lấy dữ lệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] EmployeeError employeeError)
        {
            try
            {
                if (employeeError.ID <= 0) await _employeeErrorRepo.CreateAsync(employeeError);
                else await _employeeErrorRepo.UpdateAsync(employeeError);
                return Ok(ApiResponseFactory.Success(employeeError, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
