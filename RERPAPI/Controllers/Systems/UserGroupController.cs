using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Systems;
using RERPAPI.Model.DTO;
using RERPAPI.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace RERPAPI.Controllers.Systems
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserGroupController : ControllerBase
    {

        UserGroupRepo _userGroupRepo;
        UserGroupLinkRepo _userGroupLinkRepo;
        UserGroupRightDistributionRepo _userGroupRightDistributionRepo;

        public UserGroupController(
            UserGroupRepo userGroupRepo,
            UserGroupLinkRepo userGroupLinkRepo,
            UserGroupRightDistributionRepo userGroupRightDistributionRepo)
        {
            _userGroupRepo = userGroupRepo;
            _userGroupLinkRepo = userGroupLinkRepo;
            _userGroupRightDistributionRepo = userGroupRightDistributionRepo;
        }

        // Lấy tất cả danh sách nhóm người dùng
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpGet("getall")]
        public IActionResult GetAll(string keyword = "", int userId = 0)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetUserGroup", new string[] { "@Keyword", "@UserID" }, new object[] { keyword ?? "", userId });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(datas, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lấy danh sách liên kết người dùng trong nhóm
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpGet("get-group-links")]
        public IActionResult GetGroupLinks(int userGroupId)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetUserGroupLink", new string[] { "@UserID", "@UserGroupID" }, new object[] { 0, userGroupId });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(datas, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lấy danh sách phân quyền của nhóm
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpGet("get-rights-distribution")]
        public IActionResult GetRightsDistribution(int userGroupId)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetUserGroupRightDistributionByUserGroupID", new string[] { "@UserGroupID" }, new object[] { userGroupId });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(datas, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lấy cây phân quyền của nhóm
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpGet("get-group-permission-tree")]
        public IActionResult GetGroupPermissionTree(int userGroupId)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetGroupPermission", new string[] { "@UserGroupID" }, new object[] { userGroupId });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(datas, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lấy danh sách quyền của người dùng theo tất cả nhóm
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpGet("get-user-permissions")]
        public IActionResult GetUserPermissionsByUserID(int userId)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetUserPermissionsByUserID", new string[] { "@UserID" }, new object[] { userId });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(datas, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lưu danh sách nhóm quyền cho người dùng
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpPost("save-user-group-links")]
        public IActionResult SaveUserGroupLinks(int userId, string userGroupId = "")
        {
            try
            {
                // Đảm bảo truyền chuỗi rỗng khi không có nhóm nào được chọn
                var groupIds = userGroupId ?? "";
                SQLHelper<object>.ProcedureToList("spSaveUserGroupLinks", new string[] { "@UserID", "@ListUserGroupID" }, new object[] { userId, groupIds });
                return Ok(ApiResponseFactory.Success(null, "Lưu thông tin thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lưu thông tin phân quyền cho nhóm
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpPost("save-group-permissions")]
        public async Task<IActionResult> SaveGroupPermissions([FromBody] SaveGroupPermissionRequest req)
        {
            try
            {
                if (req == null || req.Permissions == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ."));

                foreach (var item in req.Permissions)
                {
                    if (item.OldValue == item.IsChecked) continue;

                    if (item.OldValue && !item.IsChecked)
                    {
                        if (item.UserGroupRightDistributionID > 0)
                        {
                            await _userGroupRightDistributionRepo.DeleteAsync(item.UserGroupRightDistributionID);
                        }
                    }
                    else if (!item.OldValue && item.IsChecked)
                    {
                        if (item.ID > 0)
                        {
                            var newDist = new UserGroupRightDistribution
                            {
                                FormAndFunctionID = item.ID,
                                UserGroupID = req.UserGroupID
                            };
                            await _userGroupRightDistributionRepo.CreateAsync(newDist);
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Phân quyền thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Thêm danh sách người dùng vào nhóm
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpPost("add-users-to-group")]
        public IActionResult AddUsersToGroup(string userIds, int userGroupId)
        {
            try
            {
                SQLHelper<object>.ProcedureToList("spAddUserToGroupRole", new string[] { "@ListUserID", "@UserGroupID" }, new object[] { userIds, userGroupId });
                return Ok(ApiResponseFactory.Success(null, "Thêm người dùng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lưu thông tin nhóm người dùng (Thêm mới hoặc Cập nhật)
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpPost("save")]
        public async Task<IActionResult> Save(UserGroup item)
        {
            try
            {
                var validate = _userGroupRepo.Validate(item);
                if (validate.Status == 0)
                {
                    return BadRequest(validate);
                }

                if (item.ID > 0)
                {
                    await _userGroupRepo.UpdateAsync(item);
                    return Ok(ApiResponseFactory.Success(item, "Cập nhật thành công"));
                }
                else
                {
                    await _userGroupRepo.CreateAsync(item);
                    return Ok(ApiResponseFactory.Success(item, "Thêm mới thành công"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Xóa nhóm người dùng
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
            
                bool hasLinks = await _userGroupLinkRepo.ExistsAsync(x => x.UserGroupID == id);
                bool hasRights = await _userGroupRightDistributionRepo.ExistsAsync(x => x.UserGroupID == id);

                if (hasLinks || hasRights)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Nhóm quyền này đã phát sinh ở các nghiệp vụ khác nên không thể xóa được!"));
                }

                await _userGroupRepo.DeleteAsync(id);
                return Ok(ApiResponseFactory.Success(null, "Xóa thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Xóa liên kết người dùng trong nhóm
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpPost("delete-link")]
        public async Task<IActionResult> DeleteLink(int id)
        {
            try
            {
                await _userGroupLinkRepo.DeleteAsync(id);
                return Ok(ApiResponseFactory.Success(null, "Xóa liên kết thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lấy danh sách nhóm người dùng theo mã chức năng
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpGet("get-user-group")]
        public IActionResult UserGroupByFormAndFunctionCode(string functionCode)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetUserGroupByFormAndFunctionCode", new string[] { "@FormAndFunctionCode" }, new object[] { functionCode });

                var data = new
                {
                    usergroup = SQLHelper<object>.GetListData(datas, 0),
                    codes = SQLHelper<object>.GetListData(datas, 1)[0].Codes
                };
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        // Lấy danh sách nhóm theo người dùng
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpGet("get-groups-by-user")]
        public IActionResult GetGroupsByUser(int userId)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetUserGroupIdByUserID", new string[] { "@UserID" }, new object[] { userId });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(datas, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        public class CopyUserGroupRequest
        {
            public int ToUserID { get; set; }
            public List<int> RoleIDs { get; set; }
        }
        // Sao chép danh sách nhóm quyền cho người dùng
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpPost("copy-user-groups")]
        public async Task<IActionResult> CopyUserGroups([FromBody] CopyUserGroupRequest req)
        {
            try
            {
                if (req == null || req.RoleIDs == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ."));

                foreach (int roleId in req.RoleIDs)
                {
                    bool exists = await _userGroupLinkRepo.ExistsAsync(x => x.UserID == req.ToUserID && x.UserGroupID == roleId);
                    if (!exists)
                    {
                        var newLink = new UserGroupLink
                        {
                            UserID = req.ToUserID,
                            UserGroupID = roleId
                        };
                        await _userGroupLinkRepo.CreateAsync(newLink);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Copy quyền thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy cây danh sách nhóm quyền và chi tiết quyền (Nhóm -> Quyền)
        [RequiresPermission(",", permissionFunction: "userPermissionForm")]
        [HttpGet("get-tree")]
        public IActionResult GetUserGroupTree()
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetUserGroupPermissionTree", new string[] { }, new object[] { });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(datas, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // Lấy danh sách nhân viên trong nhóm quyền
        [RequiresPermission(",", permissionFunction : "userPermissionForm")]
        [HttpGet("get-employees-by-group")]
        public IActionResult GetEmployeesByGroup(int userGroupId)
        {
            try
            {
                var datas = SQLHelper<object>.ProcedureToList("spGetEmployeesByGroupId", new string[] { "@UserGroupID" }, new object[] { userGroupId });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(datas, 0), ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
