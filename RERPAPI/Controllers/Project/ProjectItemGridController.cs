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
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Project
{
    public class ProjectItemGridDTO : ProjectItem
    {
        public List<int>? UserIDs { get; set; }
        public List<int>? RelatedUserIDs { get; set; }
    }

    public class ProjectItemGridFullDTO
    {
        public ProjectItemProblem? projectItemProblem { get; set; }
        public List<ProjectItemGridDTO>? projectItems { get; set; }
        public ProjectItemFile? ProjectItemFile { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectItemGridController : ControllerBase
    {
        private readonly ProjectItemRepo _projectItemRepo;
        private readonly ProjectItemProblemRepo _projectItemProblemRepo;
        private readonly ProjectTaskEmployeeRepo _projectTaskEmployeeRepo;

        public ProjectItemGridController(
            ProjectItemRepo projectItemRepo,
            ProjectItemProblemRepo projectItemProblemRepo,
            ProjectTaskEmployeeRepo projectTaskEmployeeRepo
        )
        {
            _projectItemRepo = projectItemRepo;
            _projectItemProblemRepo = projectItemProblemRepo;
            _projectTaskEmployeeRepo = projectTaskEmployeeRepo;
        }

        /// <summary>
        /// Lấy danh sách hạng mục công việc của dự án cho bảng cây editable
        /// </summary>
        [HttpGet("get-project-item")]
        public IActionResult GetProjectItem([FromQuery] int projectID, [FromQuery] DateTime? dateStart = null, [FromQuery] DateTime? dateEnd = null)
        {
            try
            {
                object dateStartParam = dateStart.HasValue ? (object)dateStart.Value : DBNull.Value;
                object dateEndParam = dateEnd.HasValue ? (object)dateEnd.Value : DBNull.Value;

                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetProjectItemGrid",
                    new string[] { "@ProjectID", "@DateStart", "@DateEnd" },
                    new object[] { projectID, dateStartParam, dateEndParam });
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

                    // Lấy danh sách Người thực hiện (Type = 1) và Người liên quan (Type = 2) cho tất cả các task
                    var taskIds = filteredList.Select(x => Convert.ToInt32(((IDictionary<string, object>)x)["ID"])).Distinct().ToList();
                    var allTaskEmployees = taskIds.Any()
                        ? _projectTaskEmployeeRepo.GetAll(x => taskIds.Contains(x.ProjectTaskID) && (x.IsDeleted != true)).ToList()
                        : new List<ProjectTaskEmployee>();

                    var assigneeMap = allTaskEmployees.Where(x => x.Type == 1)
                        .GroupBy(x => x.ProjectTaskID)
                        .ToDictionary(g => g.Key, g => g.Select(x => x.EmployeeID).ToList());

                    var relatedMap = allTaskEmployees.Where(x => x.Type == 2)
                        .GroupBy(x => x.ProjectTaskID)
                        .ToDictionary(g => g.Key, g => g.Select(x => x.EmployeeID).ToList());

                    var resultList = new List<IDictionary<string, object>>();
                    foreach (var item in filteredList)
                    {
                        var dict = new Dictionary<string, object>((IDictionary<string, object>)item);
                        int taskId = Convert.ToInt32(dict["ID"]);

                        dict["UserIDs"] = assigneeMap.ContainsKey(taskId) && assigneeMap[taskId].Any()
                            ? assigneeMap[taskId]
                            : (dict.ContainsKey("UserID") && dict["UserID"] != null && Convert.ToInt32(dict["UserID"]) > 0 ? new List<int> { Convert.ToInt32(dict["UserID"]) } : new List<int>());

                        dict["RelatedUserIDs"] = relatedMap.ContainsKey(taskId) ? relatedMap[taskId] : new List<int>();
                        resultList.Add(dict);
                    }

                    return Ok(ApiResponseFactory.Success(resultList, "Lấy dữ liệu thành công"));
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
        public async Task<IActionResult> SaveData([FromBody] ProjectItemGridFullDTO projectItem)
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

                        if (item.UserIDs != null && item.UserIDs.Any())
                        {
                            item.UserID = item.UserIDs.First();
                        }

                        if (item.ID <= 0)
                        {
                            item.STT = _projectItemRepo.GetMaxSTT(item.ProjectID);
                            // Dùng EmployeeID làm fallback nhất quán với bảng ProjectTaskEmployee
                            if (item.UserID == null || item.UserID <= 0) item.UserID = currentUser.EmployeeID > 0 ? currentUser.EmployeeID : currentUser.ID;
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

                        // Đồng bộ Người thực hiện (ProjectTaskEmployee, Type = 1) nếu có UserIDs
                        if (item.UserIDs != null && item.ID > 0)
                        {
                            var existingAssignees = _projectTaskEmployeeRepo.GetAll(x => x.ProjectTaskID == item.ID && x.Type == 1 && (x.IsDeleted != true)).ToList();
                            var newAssigneeIds = item.UserIDs.Distinct().ToList();

                            foreach (var ass in existingAssignees)
                            {
                                if (!newAssigneeIds.Contains(ass.EmployeeID))
                                {
                                    ass.IsDeleted = true;
                                    ass.UpdatedDate = DateTime.Now;
                                    ass.UpdatedBy = currentUser.LoginName;
                                    await _projectTaskEmployeeRepo.UpdateAsync(ass);
                                }
                            }

                            foreach (var empId in newAssigneeIds)
                            {
                                if (empId > 0 && !existingAssignees.Any(x => x.EmployeeID == empId))
                                {
                                    var newAssignee = new ProjectTaskEmployee
                                    {
                                        ProjectTaskID = item.ID,
                                        EmployeeID = empId,
                                        Type = 1, // 1: Người thực hiện
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now,
                                        CreatedBy = currentUser.LoginName
                                    };
                                    await _projectTaskEmployeeRepo.CreateAsync(newAssignee);
                                }
                            }
                        }

                        // Đồng bộ Người liên quan (ProjectTaskEmployee, Type = 2) nếu có dữ liệu RelatedUserIDs
                        if (item.RelatedUserIDs != null && item.ID > 0)
                        {
                            var existingRelated = _projectTaskEmployeeRepo.GetAll(x => x.ProjectTaskID == item.ID && x.Type == 2 && (x.IsDeleted != true)).ToList();
                            var newRelatedIds = item.RelatedUserIDs.Distinct().ToList();

                            // Xóa bớt những người bỏ chọn
                            foreach (var rel in existingRelated)
                            {
                                if (!newRelatedIds.Contains(rel.EmployeeID))
                                {
                                    rel.IsDeleted = true;
                                    rel.UpdatedDate = DateTime.Now;
                                    rel.UpdatedBy = currentUser.LoginName;
                                    await _projectTaskEmployeeRepo.UpdateAsync(rel);
                                }
                            }

                            // Thêm mới những người được chọn thêm
                            foreach (var empId in newRelatedIds)
                            {
                                if (empId > 0 && !existingRelated.Any(x => x.EmployeeID == empId))
                                {
                                    var newRel = new ProjectTaskEmployee
                                    {
                                        ProjectTaskID = item.ID,
                                        EmployeeID = empId,
                                        Type = 2, // 2: Người liên quan
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now,
                                        CreatedBy = currentUser.LoginName
                                    };
                                    await _projectTaskEmployeeRepo.CreateAsync(newRel);
                                }
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
