using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeTeamController : Controller
    {
        EmployeeTeamRepo employeeTeamRepo = new EmployeeTeamRepo();

        [HttpGet]
        public IActionResult GetEmployeeTeams()
        {
            try
            {
                var dtEmployeeTeams = SQLHelper<object>.ProcedureToList("spGetEmployeeTeam",new string[] { }, new object[] { });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(dtEmployeeTeams, 0)
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


        [HttpPost]
        public async Task<IActionResult> SaveEmployeeTeam([FromBody] EmployeeTeam employeeTeam)
        {
            try
            {
                if(employeeTeam.ID <= 0)
                {
                    await employeeTeamRepo.CreateAsync(employeeTeam);
                }
                else
                {
                    await employeeTeamRepo.UpdateAsync(employeeTeam);
                }
                return Ok(new
                {
                    status = 1,
                    data = employeeTeam,
                    message = "Lưu team phòng ban thành công"
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
