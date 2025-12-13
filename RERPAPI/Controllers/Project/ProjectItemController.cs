
using Azure.Core;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Project;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param.Project;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Project;
using System.Collections.Immutable;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectItemController : ControllerBase
    {
        private readonly ProjectItemProblemRepo _projectItemProblemRepo;
        private readonly ProjectItemRepo _projectItemRepo;
        private readonly ProjectItemFileRepo _projectItemFileRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly IConfiguration _configuration;

        public ProjectItemController(
            ProjectItemProblemRepo projectItemProblemRepo,
            ProjectItemRepo projectItemRepo,
            ProjectItemFileRepo projectItemFileRepo,
            ProjectRepo projectRepo,
            IConfiguration configuration
        )
        {
            _projectItemProblemRepo = projectItemProblemRepo;
            _projectItemRepo = projectItemRepo;
            _projectItemFileRepo = projectItemFileRepo;
            _projectRepo = projectRepo;
            _configuration = configuration;
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
        [HttpPost("get-project-item-over-time")]
        public IActionResult GetProjectItemOverTime(ProjectItemRequestParam request)
        {
            try
            {
                var projectItem = SQLHelper<dynamic>.ProcedureToList("spGetProjectItem",
                    new[] { "@ProjectID", "@UserID", "@Keyword", "@Status" },
                    new object[] {  request.ProjectID       , request.UserID, request.Keyword, request.Status });
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
        [HttpPost("save-tree")]
        public async Task<IActionResult> SaveTree([FromBody] ProjectItemDTO req)
        {
            var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
            var currentUser = ObjectMapper.GetCurrentUser(claims);
            try
            {
                // var user = GetCurrentUser();
                var idMap = new Dictionary<int, int>();
                var project = _projectRepo.GetAll(x => x.ID == req.ProjectID).FirstOrDefault();
                if (project == null) return Ok(new
                {
                    status = 2,
                    message = "Không tìm thấy mã dự án!"
                });


                /* // Phân loại
                 var creates = req.ProjectItems.Where(x => x.ID <= 0 && !x.IsDeleted).ToList();
                 var updates = req.ProjectItems.Where(x => x.ID > 0 && !x.IsDeleted).ToList();
                 var deletes = req.ProjectItems.Where(x => x.ID > 0 && x.IsDeleted).ToList();*/

                // Validate
                /* var validation = Validate(creates.Concat(updates).ToList(), user);
                 if (!validation.IsValid) return BadRequest(Fail(validation.Message));*/
                if (req.DeletedIdsprojectItem != null && req.DeletedIdsprojectItem.Count > 0)
                {
                    foreach (var id in req.DeletedIdsprojectItem)
                    {
                        ProjectItem pjI = _projectItemRepo.GetByID(id);
                        if (pjI != null)
                        {
                            pjI.IsDeleted = true;
                            await _projectItemRepo.UpdateAsync(pjI);
                        }
                    }
                }
                foreach (var data in req.projectItem.Where(x => x.IsDeleted == false))
                {
                    if (!_projectItemRepo.Validate(data, out string message))
                    {
                        return Ok(new { status = 3, message });
                    }
                }
                foreach (var data in req.projectItem)
                {
                    // Xóa mềm
                    if (data.ID > 0 && data.IsDeleted == true)
                    {
                        if (!_projectItemRepo.CanDelete(data, currentUser))
                        {
                            return Ok(new { status = 3, message = "Không thể xóa!" });
                        }
                        await _projectItemRepo.UpdateAsync(data);
                    }

                    // thêm mới
                    if (data.ID < 0 && data.IsDeleted == false)
                    {
                        ProjectItem item = new ProjectItem();
                        item.Status = data.Status;
                        item.STT = data.STT;
                        item.UserID = data.UserID;
                        item.Code = data.Code;
                        item.ProjectID = req.ProjectID;
                        item.Mission = data.Mission;
                        item.PlanStartDate = data.PlanStartDate;
                        item.PlanEndDate = data.PlanEndDate;
                        item.ActualStartDate = data.ActualStartDate;
                        item.ActualEndDate = data.ActualEndDate;
                        item.Note = data.Note;
                        item.TotalDayPlan = data.TotalDayPlan ?? 0;
                        item.PercentItem = data.PercentItem ?? 0;
                        item.TypeProjectItem = data.TypeProjectItem;
                        item.PercentageActual = data.PercentageActual ?? 0;
                        item.EmployeeIDRequest = data.EmployeeIDRequest;
                        item.IsUpdateLate = data.IsUpdateLate ?? false;
                        item.ReasonLate = data.ReasonLate;
                        item.ParentID = data.ParentID;
                        item.EmployeeRequestID = data.EmployeeRequestID;
                        item.EmployeeRequestName = data.EmployeeRequestName;
                        item.ItemLate = 0;
                        item.Code = data.Code;
                        _projectItemRepo.CalculateDays(item);
                        if (item.ActualEndDate.HasValue) item.IsApproved = 2;
                        await _projectItemRepo.CreateAsync(item);
                        idMap[data.ID] = item.ID;

                    }
                    else
                    {
                        //cập nhật
                        ProjectItem item = _projectItemRepo.GetByID(data.ID);
                        _projectItemRepo.CalculateDays(item);
                        if (!_projectItemRepo.CanEdit(item, currentUser) && !currentUser.IsAdmin)
                            return Ok(new { status = 4, message = "Không có quyền sửa!" });
                        item.Status = data.Status;
                        item.STT = data.STT;
                        item.UserID = data.UserID;
                        item.Code = data.Code;
                        item.ProjectID = req.ProjectID;
                        item.Mission = data.Mission;
                        item.PlanStartDate = data.PlanStartDate;
                        item.PlanEndDate = data.PlanEndDate;
                        item.ActualStartDate = data.ActualStartDate;
                        item.ActualEndDate = data.ActualEndDate;
                        item.Note = data.Note;
                        item.TotalDayPlan = data.TotalDayPlan ?? 0;
                        item.PercentItem = data.PercentItem ?? 0;
                        item.TypeProjectItem = data.TypeProjectItem;
                        item.PercentageActual = data.PercentageActual ?? 0;
                        item.EmployeeIDRequest = data.EmployeeIDRequest;
                        item.IsUpdateLate = data.IsUpdateLate ?? false;
                        item.ReasonLate = data.ReasonLate;
                        item.ParentID = data.ParentID;
                        item.EmployeeRequestID = data.EmployeeRequestID;
                        item.EmployeeRequestName = data.EmployeeRequestName;
                        item.ItemLate = 0;
                        item.Code = data.Code;
                        if (item.ActualEndDate.HasValue && item.IsApproved < 2)
                            item.IsApproved = 2;
                        await _projectItemRepo.UpdateAsync(item);
                    }
                }
                // Update ParentID cho node con
                foreach (var data in req.projectItem)
                {
                    if (data.ID < 0 && data.IsDeleted == false)
                    {
                        if (data.ParentID < 0 && idMap.ContainsKey(data.ParentID.Value))
                        {
                            var item = _projectItemRepo.GetByID(idMap[data.ID]);
                            item.Status = data.Status;
                            item.STT = data.STT;
                            item.UserID = data.UserID;
                            item.Code = data.Code;
                            item.ProjectID = req.ProjectID;
                            item.Mission = data.Mission;
                            item.PlanStartDate = data.PlanStartDate;
                            item.PlanEndDate = data.PlanEndDate;
                            item.ActualStartDate = data.ActualStartDate;
                            item.ActualEndDate = data.ActualEndDate;
                            item.Note = data.Note;
                            item.TotalDayPlan = data.TotalDayPlan ?? 0;
                            item.PercentItem = data.PercentItem ?? 0;
                            item.TypeProjectItem = data.TypeProjectItem;
                            item.PercentageActual = data.PercentageActual ?? 0;
                            item.EmployeeIDRequest = data.EmployeeIDRequest;
                            item.IsUpdateLate = data.IsUpdateLate ?? false;
                            item.ReasonLate = data.ReasonLate;
                            item.ParentID = data.ParentID;
                            item.EmployeeRequestID = data.EmployeeRequestID;
                            item.EmployeeRequestName = data.EmployeeRequestName;
                            item.ItemLate = 0;
                            item.Code = data.Code;
                            item.ParentID = idMap[data.ParentID.Value];
                            _projectItemRepo.CalculateDays(item);
                            await _projectItemRepo.UpdateAsync(item);
                        }
                    }
                }

                await _projectItemRepo.UpdatePercent(req.ProjectID);
                await _projectItemRepo.UpdateLate(req.ProjectID);

                return Ok(ApiResponseFactory.Success(req, "Lưu thành công"));
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
        //projectItemFile
        //lay du lieu
        [HttpGet("get-project-item-file")]
        public async Task<IActionResult> GetProjectItemFile(int projectItem)
        {
            try
            {
                List<ProjectItemFile> rs = _projectItemFileRepo.GetAll(x => x.ProjectItemID == projectItem && x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(rs, "Lay du lieu file thanh cong"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-file")]
        public async Task<IActionResult> SaveProjectFile(List<ProjectItemFile> dto)
        {
            try
            {
                foreach (var item in dto)
                {
                    if (item.ID > 0)
                    {
                        await _projectItemFileRepo.UpdateAsync(item);
                    }
                    else
                    {
                        await _projectItemFileRepo.CreateAsync(item);
                    }
                }
                return Ok(ApiResponseFactory.Success(null, "Lưu file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //end
        //projectItemProblem
        //lay du lieu
        [HttpGet("get-project-item-problem")]
        public async Task<IActionResult> GetProjectItemProblem(int projectItem)
        {
            try
            {
                var rs = SQLHelper<dynamic>.ProcedureToList("spGetProjectItemProblem",
                    new[] { "ProjectItemID" },
                    new object[] { projectItem });
                var rows = SQLHelper<dynamic>.GetListData(rs, 0);
                return Ok(ApiResponseFactory.Success(rows, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //save 
        [HttpPost("save-problem")]
        public async Task<IActionResult> GetProjectItemProblem(ProjectItemProblem dto)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                ProjectItem projectItem = _projectItemRepo.GetByID(dto.ProjectItemID ?? 0);
                if (!_projectItemProblemRepo.CanEdit(projectItem, currentUser))
                {
                    return Ok(new { status = 2, message = "Bạn không có quyền sửa cho hạng mục này!" });
                }
                if (dto.ID > 0)
                {
                    await _projectItemProblemRepo.UpdateAsync(dto);
                }
                else
                {
                    await _projectItemProblemRepo.CreateAsync(dto);
                }

                return Ok(ApiResponseFactory.Success(null, "Thêm thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
