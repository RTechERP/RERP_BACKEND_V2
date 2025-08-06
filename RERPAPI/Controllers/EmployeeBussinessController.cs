using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeBussinessController : ControllerBase
    {
        EmployeeBussinessRepo employeeBussinessRepo = new EmployeeBussinessRepo();
        [HttpPost]
        public IActionResult getEmployeeBussiness(EmployeeBussinessParam param)
        {
            try
            {
                var arrParamName = new string[] { "@PageNumber", "@PageSize", "@StartDate", "@EndDate", "@Keyword", "@DepartmentID", "@IDApprovedTP", "@Status" };
                var arrParamValue = new object[] { param.pageNumber, param.pageSize, param.dateStart, param.dateEnd, param.keyWord, param.departmentId, param.idApprovedTp, param.status};
                var employeeBussiness = SQLHelper<object>.ProcedureToList("spGetEmployeeBussiness", arrParamName, arrParamValue);

                var result = SQLHelper<object>.GetListData(employeeBussiness, 0);

                return Ok(new
                {
                    status = 1,
                    data = result
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
        public async Task<IActionResult> saveEmployeeBussiness([FromBody] EmployeeBussiness employeeBussiness)
        {
            try
            {
                if(employeeBussiness.ID <= 0)
                {
                    employeeBussinessRepo.CreateAsync(employeeBussiness);
                } else
                {
                    employeeBussinessRepo.UpdateFieldsByID(employeeBussiness.ID, employeeBussiness);
                }
                return Ok(new
                {
                    status = 1,
                    message = "Lưu thành công"
                });
            } catch (Exception ex)
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
