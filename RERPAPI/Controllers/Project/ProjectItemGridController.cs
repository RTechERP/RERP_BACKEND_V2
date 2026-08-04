using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Project;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectItemGridController : ControllerBase
    {
        private readonly ProjectItemRepo _projectItemRepo;
        private readonly ProjectItemProblemRepo _projectItemProblemRepo;

        public ProjectItemGridController(
            ProjectItemRepo projectItemRepo,
            ProjectItemProblemRepo projectItemProblemRepo
        )
        {
            _projectItemRepo = projectItemRepo;
            _projectItemProblemRepo = projectItemProblemRepo;
        }

        /// <summary>
        /// Lấy danh sách hạng mục công việc của dự án cho bảng cây editable
        /// </summary>
        [HttpGet("get-project-item")]
        public IActionResult GetProjectItem([FromQuery] int projectID)
        {
            try
            {
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetProjectItem",
                    new string[] { "@ProjectID" },
                    new object[] { projectID });
                var projectItemData = SQLHelper<dynamic>.GetListData(projectItem, 0);

                if (projectItemData is IEnumerable<dynamic> items)
                {
                    var filteredList = items.Where(x =>
                    {
                        var dict = (IDictionary<string, object>)x;

                        // Kiểm tra Người giao việc (EmployeeIDRequest / EmployeeRequestID / EmployeeCreateID)
                        bool hasAssigner = false;
                        if (dict.TryGetValue("EmployeeIDRequest", out var assignerVal) && assignerVal != null && Convert.ToInt32(assignerVal) > 0)
                            hasAssigner = true;
                        else if (dict.TryGetValue("EmployeeRequestID", out assignerVal) && assignerVal != null && Convert.ToInt32(assignerVal) > 0)
                            hasAssigner = true;
                        else if (dict.TryGetValue("EmployeeCreateID", out assignerVal) && assignerVal != null && Convert.ToInt32(assignerVal) > 0)
                            hasAssigner = true;

                        // Kiểm tra Người thực hiện (UserID / ProjectEmployee)
                        bool hasAssignee = false;
                        if (dict.TryGetValue("UserID", out var userVal) && userVal != null && Convert.ToInt32(userVal) > 0)
                            hasAssignee = true;
                        else if (dict.TryGetValue("ProjectEmployee", out var empStr) && empStr != null && !string.IsNullOrWhiteSpace(empStr.ToString()))
                            hasAssignee = true;

                        return hasAssigner && hasAssignee;
                    }).ToList();

                    return Ok(ApiResponseFactory.Success(filteredList, "Lấy dữ liệu thành công"));
                }

                return Ok(ApiResponseFactory.Success(projectItemData, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Sinh mã hạng mục công việc gốc cho dự án (ProjectCode_N)
        /// </summary>
        [HttpGet("get-project-item-code")]
        public IActionResult GetProjectItemCode([FromQuery] int projectId)
        {
            try
            {
                string newCode = _projectItemRepo.GenerateProjectItemCode(projectId);
                return Ok(ApiResponseFactory.Success(newCode, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Sinh mã hạng mục công việc con (ParentCode.N)
        /// </summary>
        [HttpGet("get-child-project-item-code")]
        public IActionResult GetChildProjectItemCode([FromQuery] int parentId)
        {
            try
            {
                string newCode = _projectItemRepo.GenerateChildProjectItemCode(parentId);
                return Ok(ApiResponseFactory.Success(newCode, ""));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Lưu hàng loạt hạng mục công việc (ProjectItem) riêng cho view Editable Tree-Grid
        /// </summary>
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProjectItemFullDTO projectItem)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);

                int projectID = 0;
                if (projectItem.projectItems != null)
                {
                    var codeToIdMap = new Dictionary<string, int>();

                    foreach (var item in projectItem.projectItems)
                    {
                        projectID = item.ProjectID ?? 0;

                        if (!_projectItemRepo.Validate(item, out string mesage))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, mesage));
                        }

                        // Ánh xạ ParentID cho các công việc con mới tạo cùng lô (batch)
                        if ((item.ParentID == null || item.ParentID <= 0) && !string.IsNullOrEmpty(item.Code))
                        {
                            int lastDot = item.Code.LastIndexOf('.');
                            if (lastDot > 0)
                            {
                                string parentCode = item.Code.Substring(0, lastDot);
                                if (codeToIdMap.TryGetValue(parentCode, out int parentRealId))
                                {
                                    item.ParentID = parentRealId;
                                }
                            }
                        }

                        if (item.ID <= 0)
                        {
                            item.STT = _projectItemRepo.GetMaxSTT(item.ProjectID);
                            if (item.UserID == null || item.UserID <= 0) item.UserID = currentUser.ID;
                            item.ItemLate = 0;
                            _projectItemRepo.CalculateDays(item);
                            if (item.ActualEndDate.HasValue) item.IsApproved = 2;
                            await _projectItemRepo.CreateAsync(item);

                            if (!string.IsNullOrEmpty(item.Code))
                            {
                                codeToIdMap[item.Code] = item.ID;
                            }
                        }
                        else
                        {
                            ProjectItem data = _projectItemRepo.GetByID(item.ID);
                            item.Code = data.Code;
                            item.ItemLate = 0;
                            if (item.ActualEndDate.HasValue && item.IsApproved < 2)
                                item.IsApproved = 2;
                            _projectItemRepo.CalculateDays(item);
                            await _projectItemRepo.UpdateAsync(item);

                            if (!string.IsNullOrEmpty(item.Code))
                            {
                                codeToIdMap[item.Code] = item.ID;
                            }
                        }
                    }

                    if (projectID > 0)
                    {
                        await _projectItemRepo.UpdatePercent(projectID);
                        await _projectItemRepo.UpdateLate(projectID);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
