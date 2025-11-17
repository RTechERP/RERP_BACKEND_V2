
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO.Project;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Project;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Project;

namespace RERPAPI.Controllers.Old.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthorize]
    public class ProjectItemController : ControllerBase
    {
        private readonly ProjectItemProblemRepo _projectItemProblemRepo;
        private readonly ProjectItemRepo _projectItemRepo;
        private readonly ProjectItemFileRepo _projectItemFileRepo;

        public ProjectItemController(
            ProjectItemProblemRepo projectItemProblemRepo,
            ProjectItemRepo projectItemRepo,
            ProjectItemFileRepo projectItemFileRepo
        )
        {
            _projectItemProblemRepo = projectItemProblemRepo;
            _projectItemRepo = projectItemRepo;
            _projectItemFileRepo = projectItemFileRepo;
        }
        //API lấy list hạng mục công việc 
        [ApiKeyAuthorize]
        [HttpPost("get-project-item")]
        public IActionResult GetProjectItem(ProjectItemRequestParam request)
        {
            try
            {
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetProjectItem",
                    new[] { "DateStart", "DateEnd", "@ProjectID", "@UserID", "@Keyword", "@Status" },
                    new object[] { request.DateStart, request.DateEnd, request.ProjectID, request.UserID, request.Keyword, request.Status });
                var rows = SQLHelper<dynamic>.GetListData(projectItem, 0);
                return Ok(ApiResponseFactory.Success(rows, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //Hàm lấy mã hạng mục công việc
        [ApiKeyAuthorize]
        [HttpGet("get-project-item-code")]
        public IActionResult GetProjectItemCode([FromQuery] int projectId)
        {
            try
            {
                string newCode = _projectItemRepo.GenerateProjectItemCode(projectId);
                var data = newCode;
                return Ok(ApiResponseFactory.Success(newCode, "Lấy mã thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //Hàm lưu dữ liệu
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProjectItemFullDTO dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                bool isTBP = currentUser.EmployeeID == 54;
                bool isPBP = currentUser.PositionCode == "CV57" || currentUser.PositionCode == "CV28";
                // 1) Hạng mục
                var parentIdMapping = new Dictionary<int, int>();
                if (dto.projectItems != null)
                {
                    foreach (var item in dto.projectItems)
                    {
                        int idOld = item.ID;
                        int parentId = 0;
                        if (item.ParentID.HasValue && parentIdMapping.ContainsKey(item.ParentID.Value))
                        {
                            parentId = parentIdMapping[item.ParentID.Value];
                        }
                        ProjectItem model = idOld > 0 ? _projectItemRepo.GetByID(idOld) : new ProjectItem();
                        model.ProjectID = item.ProjectID;
                        model.ParentID = parentId;
                        model.Status = item.Status;
                        model.STT = item.STT ?? _projectItemRepo.GetMaxSTT(item.ProjectID);
                        model.Mission = item.Mission;
                        model.PlanStartDate = item.PlanStartDate;
                        model.PlanEndDate = item.PlanEndDate;
                        model.ActualStartDate = item.ActualStartDate;
                        model.ActualEndDate = item.ActualEndDate;
                        model.Note = item.Note;
                        model.TotalDayPlan = item.TotalDayPlan;
                        model.PercentItem = item.PercentItem;
                        model.UserID = item.UserID;
                        model.ParentID = parentId > 0 ? parentId : null;
                        model.TotalDayActual = item.TotalDayActual;
                        model.ItemLate = item.ItemLate;
                        model.TimeSpan = item.TimeSpan;
                        model.TypeProjectItem = item.TypeProjectItem;
                        model.PercentageActual = item.PercentageActual;
                        model.EmployeeIDRequest = item.EmployeeIDRequest;
                        model.UpdatedDateActual = item.UpdatedDateActual;
                        model.IsApproved = item.IsApproved;
                        model.Code = item.Code;
                        model.IsUpdateLate = item.IsUpdateLate;
                        model.ReasonLate = item.ReasonLate;
                        model.UpdatedDateReasonLate = item.UpdatedDateReasonLate;
                        model.EmployeeRequestID = item.EmployeeRequestID;
                        model.EmployeeRequestName = item.EmployeeRequestName;
                        ProjectItem? existing = item.ID > 0 ? _projectItemRepo.GetByID(item.ID) : null;
                        int approved = existing?.IsApproved ?? 0;
                        if (idOld > 0)
                        {
                            await _projectItemRepo.UpdateAsync(model);
                        }
                        else
                        {
                            await _projectItemRepo.CreateAsync(model);
                        }
                        parentIdMapping.Add(item.ID, model.ID);

                        // Xóa mềm
                        if (item.IsDeleted == true && item.ID != 0)
                        {
                            if (!(currentUser.IsAdmin || isTBP || isPBP))
                                return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền xóa hạng mục"));
                            if (approved > 0)
                                return BadRequest(ApiResponseFactory.Fail(null, "Hạng mục đã duyệt không thể xóa"));

                            if (dto.projectItemProblem != null)
                            {
                                if (dto.projectItemProblem.ID <= 0)
                                {
                                    await _projectItemProblemRepo.CreateAsync(dto.projectItemProblem);
                                }
                                else
                                {
                                    _projectItemProblemRepo.Update(dto.projectItemProblem);
                                }
                            }
                            //// 3) File--Tạm thời chưa cần
                            //if (dto.ProjectItemFile != null)
                            //{
                            //    if (dto.ProjectItemFile.ID <= 0)
                            //    {
                            //        await _projectItemFileRepo.CreateAsync(dto.ProjectItemFile);
                            //    }
                            //    else
                            //    {
                            //        _projectItemFileRepo.Update(dto.ProjectItemFile);
                            //    }
                            //}

                            // 4) Tính lại % theo TotalDayPlan cho toàn bộ Project
                            int projectId = dto.projectItems?.FirstOrDefault()?.ProjectID ?? 0;
                            if (projectId > 0)
                            {
                                var items = _projectItemRepo.GetAll(x => x.ProjectID == projectId && x.IsDeleted == false);
                                decimal total = items.Sum(x => x.TotalDayPlan ?? 0m);
                                foreach (var it in items)
                                {
                                    var plan = it.TotalDayPlan ?? 0m;

                                    it.PercentItem = total > 0 ? plan / total * 100m : 0m;

                                    _projectItemRepo.Update(it);
                                }
                            }
                        }
                    
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu hạng mục thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //[HttpPost("save-data")]
        //public async Task<IActionResult> SaveData([FromBody] ProjectItemFullDTO dto)
        //{
        //    try
        //    {
        //        // Your existing code logic here
        //        return Ok(ApiResponseFactory.Success(null, "Lưu hạng mục thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}
    }
}
