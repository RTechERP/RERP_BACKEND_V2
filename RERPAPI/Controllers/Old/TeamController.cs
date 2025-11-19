using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
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

        private readonly TeamRepo teamRepo;
        private readonly UserTeamRepo _userTeamRepo;
        private readonly UserTeamLinkRepo _userTeamLinkRepo;
        public TeamController(UserTeamRepo userTeamRepo, UserTeamLinkRepo userTeamLinkRepo, TeamRepo teamRepo)
        {
            _userTeamRepo = userTeamRepo;
            _userTeamLinkRepo = userTeamLinkRepo;
            this.teamRepo = teamRepo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var teams = _userTeamRepo.GetAll();
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
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        [HttpPost]
        public async Task<IActionResult> SaveData([FromBody] UserTeam userTeam)
        {
            try
            {
                List<UserTeam> userTeams = _userTeamRepo.GetAll();
                if (userTeams.Any(x => x.Name == userTeam.Name && x.ID != userTeam.ID))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Tên team đã tồn tại"));
                }
                if (userTeam.ID <= 0)
                {
                    await _userTeamRepo.CreateAsync(userTeam);
                }
                else
                {
                    await _userTeamRepo.UpdateAsync(userTeam);
                }
                return Ok(ApiResponseFactory.Success("", "Cập nhật thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("{teamID}")]
        public async Task<IActionResult> DeleteTeam(int teamID)
        {
            try
            {
                if(teamID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không được xóa team cha cao nhất!"));
                }

                var team = _userTeamRepo.GetByID(teamID);
                if (team == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Team không tồn tại"));
                }
                team.IsDeleted = true;
                await _userTeamRepo.UpdateAsync(team);
                return Ok(ApiResponseFactory.Success("", "Xóa team thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }


        [HttpPost("add-employee")]
        public async Task<IActionResult> AddEmployeeToTeam([FromBody] AddEmployeeToTeamRequest request)
        {
            try
            {
                if (request == null || request.ListEmployeeID == null || !request.ListEmployeeID.Any())
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách ID nhân viên không hợp lệ hoặc rỗng."));
                }

                if (request.TeamID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "ID team không hợp lệ."));
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("remove-employee")]
        public async Task<IActionResult> RemoveEmployeeFromTeam(int userTeamLinkID)
        {
            try
            {
                var userTeamLink = _userTeamLinkRepo.GetByID(userTeamLinkID);
                _userTeamLinkRepo.Delete(userTeamLink.ID);
                return Ok(new
                {
                    status = 1,
                    message = "Xóa nhân viên khỏi team thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
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
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
