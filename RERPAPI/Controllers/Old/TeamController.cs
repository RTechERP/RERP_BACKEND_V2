using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        public class AddEmployeeToTeamRequest
        {
            public int TeamID { get; set; }
            public List<int> ListEmployeeID { get; set; }
        }

        TeamRepo teamRepo = new TeamRepo();
        UserTeamRepo userTeamRepo = new UserTeamRepo();
        UserTeamLinkRepo userTeamLinkRepo = new UserTeamLinkRepo();

        [HttpGet]
        public IActionResult GetAll()
        {
            var teams = teamRepo.GetAll();
            return Ok(teams);
        }

        [HttpGet("department/{departmentID}")]
        public IActionResult GetTeamByDepartmentID(int departmentID)
        {
           try
            {
                var teams = SQLHelper<object>.ProcedureToList("spGetTreeUserTeamData", new string[] { "@DepartmentID" }, new object[] { departmentID });
                return Ok(new
                {
                    status = 1,
                    data = SQLHelper<object>.GetListData(teams, 0)
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

        [HttpGet("user-team")]
        public IActionResult GetUserTeam(int teamID, int departmentID)
        {
            try
            {
                var teams = SQLHelper<dynamic>.ProcedureToList("spGetUserTeamLink_New", new string[] { "@UserTeamID", "@DepartmentID" }, new object[] { teamID, departmentID });
                return Ok(new
                {
                    status = 1,
                    data = teams[0]
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
        public async Task<IActionResult> SaveData([FromBody] UserTeam userTeam)
        {
            try
            {
                List<UserTeam> userTeams = userTeamRepo.GetAll();
                if (userTeams.Any(x => x.Name == userTeam.Name && x.ID != userTeam.ID))
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Tên team đã tồn tại"
                    });
                }
                if (userTeam.ID <= 0)
                {
                    await userTeamRepo.CreateAsync(userTeam);
                } else
                {
                    await userTeamRepo.UpdateAsync(userTeam);
                }
                return Ok(new
                {
                    status = 1,
                    data = userTeam,
                    message = "Cập nhật thành công"
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

        [HttpDelete("{teamID}")]
        public IActionResult DeleteTeam(int teamID)
        {
            try
            {
                var team = userTeamRepo.GetByID(teamID);
                if (team == null)
                {
                    return NotFound(new
                    {
                        status = 0,
                        message = "Team không tồn tại"
                    });
                }
                userTeamRepo.Delete(team.ID);
                return Ok(new
                {
                    status = 1,
                    message = "Xóa team thành công."
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


        [HttpPost("add-employee")]
        public async Task<IActionResult> AddEmployeeToTeam([FromBody] AddEmployeeToTeamRequest request)
        {
            try
            {
                if (request == null || request.ListEmployeeID == null || !request.ListEmployeeID.Any())
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "Danh sách ID nhân viên không hợp lệ hoặc rỗng."
                    });
                }

                if (request.TeamID <= 0)
                {
                    return BadRequest(new
                    {
                        status = 0,
                        message = "ID team không hợp lệ."
                    });
                }

                string lstID = string.Join(";", request.ListEmployeeID);

                var result = SQLHelper<dynamic>.ProcedureToList(
                    "spAddUserToUserTeamLink",
                    new string[] { "@ListUserID", "@UserTeamID" },
                    new object[] { lstID, request.TeamID });

                // Kiểm tra kết quả (tùy thuộc vào stored procedure trả về)
                // Giả sử stored procedure không trả về dữ liệu, chỉ thực hiện insert
                return Ok(new
                {
                    status = 1,
                    message = "Thêm nhân viên vào team thành công."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 0,
                    message = "Đã xảy ra lỗi khi thêm nhân viên vào team."
                });
            }
        }

        [HttpDelete("remove-employee")]
        public async Task<IActionResult> RemoveEmployeeFromTeam(int userTeamLinkID)
        {
            try
            {
                var userTeamLink = userTeamLinkRepo.GetByID(userTeamLinkID);
                userTeamLinkRepo.Delete(userTeamLink.ID);
                return Ok(new
                {
                    status = 1,
                    message = "Xóa nhân viên khỏi team thành công."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 0,
                    message = "Đã xảy ra lỗi khi xóa nhân viên khỏi team."
                });
            }
        }

        [HttpGet("employee-by-department")]
        public IActionResult GetEmployeeByDepartmentID(int departmentID, int userTeamID)
        {
            try
            {
                var employees = SQLHelper<dynamic>.ProcedureToList("spGetEmployeeByDepartmentID_New", new string[] { "@DepartmentID", "@UserTeam" }, new object[] { departmentID, userTeamID });
                return Ok(new
                {
                    status = 1,
                    data = employees[0]
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
