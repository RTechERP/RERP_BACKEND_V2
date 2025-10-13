using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeEducationLevelController : ControllerBase
    {
        EmployeeEducationLevelRepo employeeEducationLevelRepo = new EmployeeEducationLevelRepo();
        [HttpGet("{id}")]
        public IActionResult GetEmployeeEducationLevelByEmployeeID(int id)
        {
            try
            {
                var employeeEducationLevel = SQLHelper<object>.ProcedureToList("spGetEmployeeEduLevel", new string[] { "@EmployeeID" }, new object[] { id });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(employeeEducationLevel, 0)
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
    }
}
