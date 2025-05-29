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

        DepartmentRepo departmentRepo = new DepartmentRepo();
        EmployeeRepo employeeRepo = new EmployeeRepo();

        [HttpGet("getall")]
        public IActionResult GetAll(int yearValue, int quarterValue, int departmentID)
        {

            try
            {

                var teams = SQLHelper<object>.ProcedureToList("spGetALLKPIEmployeeTeam",
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
        [HttpGet("getemployeeinteam")]
        public IActionResult GetAllEmployee(int departmentID = 0, int kpiEmployeeTeamID = 0)
        {
            try
            {
                var employees = SQLHelper<object>.ProcedureToList("spGetKPIEmployeeByDepartmentID", new string[] { "@DepartmentID", "@KPIEmployeeTeam" }, new object[] { departmentID, kpiEmployeeTeamID });

                return Ok(new { status = 1, data = SQLHelper<object>.GetListData(employees,0) });
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
        [HttpGet("getbyid")]
        public IActionResult FindByID(int id)
        {
            KPIEmployeeTeam team = teamRepo.GetByID(id);
            if (team == null)
            {
                return Ok(new { status = 0, message = "Team không có trong cơ sở dữ liệu!" });
            }
            return Ok(new { status = 1, data = team });
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
