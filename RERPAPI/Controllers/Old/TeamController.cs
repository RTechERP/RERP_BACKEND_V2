using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
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
        [RequiresPermission("N26,N40,N1")]
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
        [RequiresPermission("N26,N40,N1")]
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
        [RequiresPermission("N26,N40,N1")]
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
                    // Thêm mới nhóm
                    await _userTeamRepo.CreateAsync(userTeam);
                }
                else
                {
                    // Cập nhật nhóm đã có
                    var oldTeam = _userTeamRepo.GetByID(userTeam.ID);
                    int? oldDeptID = oldTeam?.DepartmentID;

                    await _userTeamRepo.UpdateAsync(userTeam);

                    // Nếu phòng ban thay đổi thì cập nhật đệ quy cho các nhóm con cấp dưới
                    if (userTeam.DepartmentID != oldDeptID)
                    {
                        await UpdateDescendants(userTeam.ID, null, userTeam.DepartmentID);
                    }
                }
                return Ok(ApiResponseFactory.Success("", "Cập nhật thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("{teamID}")]
        [RequiresPermission("N26,N40,N1")]
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
                // Đánh dấu xóa nhóm hiện tại
                team.IsDeleted = true;
                await _userTeamRepo.UpdateAsync(team);

                // Tự động đánh dấu xóa cho toàn bộ các nhóm con cấp dưới
                await UpdateDescendants(teamID, true, null);

                return Ok(ApiResponseFactory.Success("", "Xóa team thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }


        [HttpPost("add-employee")]
        [RequiresPermission("N26,N40,N1")]
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
                return Ok(ApiResponseFactory.Success("", "Thêm nhân viên vào team thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpDelete("remove-employee")]
        [RequiresPermission("N26,N40,N1")]
        public async Task<IActionResult> RemoveEmployeeFromTeam(int userTeamLinkID)
        {
            try
            {
                var userTeamLink = _userTeamLinkRepo.GetByID(userTeamLinkID);
                _userTeamLinkRepo.Delete(userTeamLink.ID);
                return Ok(ApiResponseFactory.Success("", "Xóa nhân viên khỏi team thành công."));
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
                return Ok(ApiResponseFactory.Success(employees[0], ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-team-by-department-id")]
        public IActionResult getTeamByDepartment(int departmentID)
        {

            try
            {
                var data = _userTeamRepo.GetAll(x => x.DepartmentID == departmentID);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công!"));
            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        /// <summary>
        /// Cập nhật đệ quy các nhóm con khi nhóm cha thay đổi phòng ban hoặc bị xóa
        /// </summary>
        private async Task UpdateDescendants(int parentID, bool? isDeleted, int? departmentID)
        {
            // Lấy toàn bộ danh sách nhóm chưa bị xóa để duyệt cây
            var allTeams = _userTeamRepo.GetAll(x => x.IsDeleted == false || x.IsDeleted == null);
            var descendants = new List<UserTeam>();
            
            // Tìm tất cả các nhóm con, cháu... trực thuộc nhóm cha này
            GetDescendants(parentID, allTeams, descendants);

            if (descendants.Any())
            {
                // Cập nhật trạng thái xóa hoặc phòng ban cho từng nhóm con tìm được
                foreach (var d in descendants)
                {
                    if (isDeleted.HasValue) d.IsDeleted = isDeleted.Value;
                    if (departmentID.HasValue) d.DepartmentID = departmentID.Value;
                }
                // Lưu thay đổi hàng loạt xuống database
                await _userTeamRepo.UpdateRangeAsync(descendants);
            }
        }

        /// <summary>
        /// Hàm hỗ trợ tìm kiếm đệ quy tất cả các nhóm cấp dưới
        /// </summary>
        private void GetDescendants(int parentID, List<UserTeam> allTeams, List<UserTeam> result)
        {
            // Lấy các nhóm con trực tiếp
            var children = allTeams.Where(x => x.ParentID == parentID).ToList();
            foreach (var child in children)
            {
                result.Add(child);
                // Tiếp tục tìm các nhóm con của nhóm này
                GetDescendants(child.ID, allTeams, result);
            }
        }
    }
}
