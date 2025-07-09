using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeEarlyLateController : Controller
    {
        EmployeeEarlyLateRepo employeeEarlyLateRepo = new EmployeeEarlyLateRepo();
        [HttpPost]
        public IActionResult GetEmployeeEarlyLate(EmployeeEarlyLateParam param)
        {
            try
            {
                var arrParamName = new string[] { "@FilterText", "@PageNumber", "@PageSize", "@Month", "@Year", "@DepartmentID", "@IDApprovedTP", "@Status" };
                var arrParamValue = new object[] { param.filterText, param.pageNumber, param.pageSize, param.month, param.year, param.departmentId, param.idApprovedTp, param.status};
                var employeeEarlyLate = SQLHelper<object>.ProcedureToList("spGetEmployeeEarlyLate", arrParamName, arrParamValue);
                return Ok(new {
                    data = SQLHelper<object>.GetListData(employeeEarlyLate, 0),
                    status = 1
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveEmployeeEarlyLate([FromBody] EmployeeEarlyLate employeeEarlyLate)
        {
            try
            {
               if(employeeEarlyLate.ID <= 0)
                {
                    var result = await employeeEarlyLateRepo.CreateAsync(employeeEarlyLate);
                    return Ok(new
                    {
                        status = 1,
                        message = "Thêm thành công",
                        data = result
                    });
                }
                else
                {
                    var result = employeeEarlyLateRepo.UpdateFieldsByID(employeeEarlyLate.ID, employeeEarlyLate);
                    return Ok(new
                    {
                        status = 1,
                        message = "Cập nhật thành công",
                        data = result
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
