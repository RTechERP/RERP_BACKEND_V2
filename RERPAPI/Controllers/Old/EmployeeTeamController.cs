using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeTeamController : Controller
    {
        private readonly EmployeeTeamRepo _employeeTeamRepo;
        private readonly EmployeeRepo _employeeRepo;

        public EmployeeTeamController(EmployeeTeamRepo employeeTeamRepo, EmployeeRepo employeeRepo)
        {
            _employeeTeamRepo = employeeTeamRepo;
            _employeeRepo = employeeRepo;
        }

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
        [RequiresPermission("N1,N2,N60")]
        public async Task<IActionResult> SaveEmployeeTeam([FromBody] EmployeeTeam employeeTeam)
        {
            try
            {
                if(employeeTeam.ID <= 0)
                {
                    await _employeeTeamRepo.CreateAsync(employeeTeam);
                }
                else
                {
                    await _employeeTeamRepo.UpdateAsync(employeeTeam);
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
        [HttpGet("update-employee-team")]
        [RequiresPermission("N1,N2,N60")]
        public async Task<IActionResult> UpdateEmployeeTeam(int employeeID, int teamID)
        {
            try
            {
                var employee = _employeeRepo.GetByID(employeeID);
                if (employee == null)
                {
                    return BadRequest(new { status = 0, message = "Không tìm thấy nhân viên" });
                }

                employee.EmployeeTeamID = teamID == 0 ? null : teamID;
                await _employeeRepo.UpdateAsync(employee);

                return Ok(new
                {
                    status = 1,
                    message = teamID == 0 ? "Xóa nhân viên khỏi team thành công" : "Cập nhật team cho nhân viên thành công"
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
