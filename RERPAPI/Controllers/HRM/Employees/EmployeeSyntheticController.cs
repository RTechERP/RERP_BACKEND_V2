using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.HRM.Employees
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeSyntheticController : ControllerBase
    {

        [HttpGet("get-employee-synthetic")]
        public IActionResult GetEmployeeSynthetic(int month = 0, int year = 0, int departmentId = 0, int employeeId = 0)
        {
            try
            {
                // Sử dụng SQLHelper cho stored procedure như trong projectSumary.txt
                var result = SQLHelper<object>.ProcedureToList("spGetEmployeeSyntheticByMonth",
                    new string[] { "@Month", "@Year", "@DepartmentID", "@EmployeeID" },
                    new object[] { month == 0 ? DateTime.Now.Month : month,
                                  year == 0 ? DateTime.Now.Year : year,
                                  departmentId, employeeId });

                var data = SQLHelper<object>.GetListData(result, 0);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
