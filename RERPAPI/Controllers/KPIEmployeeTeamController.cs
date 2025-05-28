using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Threading.Tasks;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIEmployeeTeamController : ControllerBase
    {
        KPIEmployeeTeamRepo teamRepo = new KPIEmployeeTeamRepo();
        KPIEmployeeTeamLinkRepo teamLinkRepo = new KPIEmployeeTeamLinkRepo();
        DepartmentRepo departmentRepo = new DepartmentRepo();
        EmployeeRepo employeeRepo = new EmployeeRepo();

        [HttpGet("getall")]
        public IActionResult GetAll(int yearValue, int quarterValue, int departmentID)
        {

            try
            {

                var teams = SQLHelper<object>.ProcedureToDynamicLists("spGetALLKPIEmployeeTeam",
                                                                new string[] { "@YearValue", "@QuarterValue", "@DepartmentID" },
                                                                new object[] { yearValue, quarterValue, departmentID });


                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(teams, 0)
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }

        }


        [HttpGet("getkpiemployeeteamlink")]
        public IActionResult GetKPIEmployeeTeamLink(int kpiEmployeeteamID, int departmentID, int yearValue, int quarterValue)
        {
            try
            {
                var teamlinks = SQLHelper<object>.ProcedureToDynamicLists("spGetKPIEmployeeTeamLink_New",
                                                                new string[] { "@KPIEmployeeteamID", "@DepartmentID", "@YearValue", "@QuarterValue" },
                                                                new object[] { kpiEmployeeteamID, departmentID, yearValue, quarterValue });


                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(teamlinks, 0)
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }


        [HttpPost("savedata")]
        public async Task<IActionResult> SaveData([FromBody] KPIEmployeeTeam team)
        {
            try
            {

                if (team.ID <= 0) await teamRepo.CreateAsync(team);
                else teamRepo.UpdateFieldsByID(team.ID, team);

                return Ok(new
                {
                    status = 1,
                    data = team,
                    message = "Cập nhật thành công!"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
