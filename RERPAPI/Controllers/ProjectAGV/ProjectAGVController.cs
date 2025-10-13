//using Azure.Core;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using RERPAPI.Model.Common;
//using RERPAPI.Model.Context;
//using RERPAPI.Model.DTO;
//using RERPAPI.Model.DTO.Project;
//using RERPAPI.Model.DTO.ProjectAGV;
//using RERPAPI.Model.Entities;
//using RERPAPI.Model.Param;
//using RERPAPI.Model.Param.Project;
//using RERPAPI.Model.Param.ProjectAGV;
//using RERPAPI.Repo.GenericEntity;
//using RERPAPI.Repo.GenericEntity.Project;
//using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
//using System.Drawing;
//using System;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers.ProjectAGV
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectAGVController : ControllerBase
    {
        //API Get Projects
        ProjectRepo _projectRepo = new ProjectRepo();
        ProjectTypeRepo _projectTypeRepo = new ProjectTypeRepo();
        ProjectStatusRepo _projectStatusRepo = new ProjectStatusRepo();
        ProjectItemRepo _projectItemRepo = new ProjectItemRepo();
        readonly ProjectIssuesRepo _projectIssuesRepo = new ProjectIssuesRepo();
        readonly ProjectDocumentsRepo _projectDocumentsRepo = new ProjectDocumentsRepo();
        RoleConfig _roleConfig = new RoleConfig();
        //API lấy danh sách tàI liệu dự án 
        [HttpPost("get-project-issues")]
        public async Task<ActionResult> spGetProjectIssues([FromBody] ProjectIssuesRequestParam request)
        {
            try
            {
                string procedureName = "spGetProjectIssues";
                string[] paramNames = new string[] { "@DateStart", "@DateEnd", "@ProjectID", "@DepartmentID", "@Keyword", "@EmployeeID" };
                object[] paramValues = new object[] { request.DateStart ?? new DateTime(2000, 01, 01), request.DateEnd ?? new DateTime(2099, 01, 01), request.ProjectID ?? 0, request.DepartmentID ?? 0, request.Keyword ?? "", request.EmployeeID ?? 0 };
                var data = SQLHelper<ProjectIssueDTO>.ProcedureToListModel(procedureName, paramNames, paramValues);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //API lưu tài liệu dự án
        [HttpPost("save-project-issues")]
        public async Task<IActionResult> SaveProjectIssues([FromBody] ProjectIssueDTO projectIssueDTO)
        {
            try
            {
                if (!_projectIssuesRepo.Validate(projectIssueDTO, out var error1))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, error1));
                }
                ProjectIssues projectIssue = _projectIssuesRepo.MapFromProjectIssueDTOToFromProject(projectIssueDTO);
                if (projectIssue.ID == null || projectIssue.ID <= 0)
                {
                    if (!_projectIssuesRepo.Validate(projectIssueDTO, out var error))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, error));
                    }
                    projectIssue.CreatedDate = DateTime.Now;
                    projectIssue.UpdatedDate = DateTime.Now;
                    await _projectIssuesRepo.CreateAsync(projectIssue);
                    return Ok(ApiResponseFactory.Success(null, "Thêm thành công"));
                }
                else
                {
                    if (!_projectIssuesRepo.Validate(projectIssueDTO, out var error))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, error));
                    }
                    projectIssue.UpdatedDate = DateTime.Now;
                    await _projectIssuesRepo.UpdateAsync(projectIssue);
                    return Ok(ApiResponseFactory.Success(null, "Sửa thành công"));
                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    message = ex.Message,
                    Status = 0
                });
            }
        }
        [HttpPost("get-project-document")]
        public IActionResult GetProjectDocument([FromBody] ProjectDocumentRequestParam req)
        {
            try
            {
                var (names, values) = req.ToSqlParams();
                var ds = SQLHelper<ProjectDocumentDTO>.ProcedureToListModel("spGetProjectDocument", names, values);
                return Ok(ApiResponseFactory.Success(ds, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-project-document")]
        public async Task<IActionResult> SaveProjectDocument([FromBody] List<ProjectDocumentDTO> documents)
        {
            try
            {
                // Validate input
                if (documents == null || !documents.Any())
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách tài liệu không được để trống"));
                }


//                foreach (ProjectDocumentDTO item in documents)
//                {
//                    // Xử lý xóa mềm trước
//                    if (item.IsDeleted == true && item.ID > 0)
//                    {
//                        ProjectDocument deleteModel = _projectDocumentsRepo.GetByID(item.ID.Value);
//                        if (deleteModel != null)
//                        {
//                            deleteModel.IsDeleted = true;
//                            _projectDocumentsRepo.PrepareForSave(deleteModel, false);
//                            await _projectDocumentsRepo.UpdateAsync(deleteModel);
//                        }
//                        continue; // Skip đến item tiếp theo
//                    }

//                    if (!_projectDocumentsRepo.ValidateForSaveDTO(item, out string message1))
//                    {
//                        return BadRequest(ApiResponseFactory.Fail(null, message1));
//                    }
//                    int id = item.ID ?? 0;
//                    ProjectDocument model = id > 0 ? _projectDocumentsRepo.GetByID(id) : new ProjectDocument();
//                    model.ProjectID = item.ProjectID ?? 0;
//                    model.Name = item.Name;
//                    model.Type = item.Type ?? 0;
//                    model.FilePath = item.FilePath;
//                    model.Version = item.Version;
//                    model.Size = item.Size;
//                    model.CreateBy = item.CreateBy;
//                    model.UpdatedBy = item.UpdatedBy;
//                    model.IsDeleted = false;

//                    //// Validate sử dụng repo validation
//                    //if (!_projectDocumentsRepo.ValidateForSave(model, out string message))
//                    //{
//                    //    return BadRequest(ApiResponseFactory.Fail(null, message));
//                    //}

                    if (id > 0)
                    {
                        // Sửa: ID > 0
                        model.ID = id;
                        // _projectDocumentsRepo.PrepareForSave(model, false); // isInsert = false
                        await _projectDocumentsRepo.UpdateAsync(model);
                    }
                    else
                    {

                        //    _projectDocumentsRepo.PrepareForSave(model, true); // isInsert = true
                        await _projectDocumentsRepo.CreateAsync(model);
                    }
                }

                return Ok(ApiResponseFactory.Success(true, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-projects")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjects([FromBody] ProjectAGVRequestParam request)
        {
            try
            {
                int[] typeCheck = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                List<int> projectTypeIDs = _projectTypeRepo.GetAll().Select(x => x.ID).ToList();

                if (string.IsNullOrWhiteSpace(projectType)) projectType = string.Join(",", projectTypeIDs);
                else
                {
                    foreach (string item in projectType.Split(','))
                    {
                        //new object[] {
                        //    request.Size, request.Page, request.DateStart??new DateTime(2000-1-1), request.DateEnd??new DateTime(2099-1-1), request.FilterText ?? "", request.CustomerID??0, request.LeaderID??0, "1,2,3,4,5,6,7,8,9,10,11",
                        //    request.UserTechID??0, request.PmID??0, typeCheck[0] ,typeCheck[1] ,typeCheck[2] ,typeCheck[3] ,typeCheck[4] ,typeCheck[5]
                        //    ,typeCheck[6] ,typeCheck[7] ,typeCheck[8], request.GlobalUserID??0, request.BussinessFieldID??0, true
                        //});
                        new object[] {
                          request.Size, request.Page, request.DateStart, request.DateEnd, request.FilterText ?? "", request.CustomerID, request.SaleID, request.ProjectType??"1,2,3,4,5,6,7,8,9,10,11", request.LeaderID,
                          request.UserTechID, request.PmID, typeCheck[0] ,typeCheck[1] ,typeCheck[2] ,typeCheck[3] ,typeCheck[4] ,typeCheck[5]
                          ,typeCheck[6] ,typeCheck[7] ,typeCheck[8], request.GlobalUserID, request.BussinessFieldID, true
                    });
                        foreach (var item in project)
                        {

                            var lsttask = _projectItemRepo.GetAll(x => x.ProjectID == item.ID);
                            var tasks = lsttask.Select(x => new ProjectItemDTO
                            {
                                ID = x.ID,
                                ProjectID = x.ProjectID ?? 0,
                                Name = "",
                                Status = x.Status,
                                Mission = x.Mission,
                                TypeProjectItem = x.TypeProjectItem,
                                Priority = 0,
                                PercentItem = x.PercentItem,
                                Steps = "",
                                PlanStartDate = x.PlanStartDate,
                                PlanEndDate = x.PlanEndDate,
                                ActualEndDate = x.ActualEndDate,
                                PICEmployeeCode = "",
                                Supports = "",
                                FilePaths = "",
                                CreatedDate = x.CreatedDate,
                                UpdatedDate = x.UpdatedDate,
                                IsDeleted = x.IsDeleted,
                                UserID = x.UserID
                            }).ToList();
                            item.Tasks = tasks;
                            var lstIssues = _projectIssuesRepo.GetAll(i => i.ProjectID == item.ID);

                            var issues = lstIssues.Select(i => new ProjectIssueDTO
                            {
                                ID = i.ID,
                                ProjectID = i.ProjectID,
                                Title = i.Title,
                                Description = i.Description,
                                Probability = (byte?)i.Probability,
                                Impact = (byte?)i.Impact,
                                Status = (byte?)i.Status,
                                Solution = i.Solution,
                                MitigationPlan = i.MitigationPlan,
                                //EmployeeCode = i.EmployeeCode,         
                                FilePath = i.FilePath,
                                CreatedDate = i.CreatedDate,
                                UpdatedDate = i.UpdatedDate
                            }).ToList();

                            item.Issues = issues;
                            var lstDocument = _projectDocumentsRepo.GetAll(d => d.ProjectID == item.ID && d.IsDeleted == false);


                            var documents = lstDocument.Select(d => new ProjectDocumentDTO
                            {
                                ID = d.ID,
                                ProjectID = d.ProjectID,
                                Name = d.Name,
                                Type = (byte?)d.Type,
                                FilePath = d.FilePath,
                                Version = d.Version,
                                Size = d.Size,
                                CreateBy = d.CreateBy,
                                CreateDate = d.CreateDate,
                                UpdatedBy = d.UpdatedBy,
                                UpdatedDate = d.UpdatedDate,
                                IsDeleted = d.IsDeleted
                            }).ToList();

                            item.Documents = documents;

                        }

                        return Ok(ApiResponseFactory.Success(project, ""));
                    }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //API Save dự án
        [HttpPost("save-project-agv")]
        public async Task<IActionResult> SaveProjectAGV([FromBody] ProjectAGVResponseDTO dto)
        {
            try
            {

//                if (!_projectRepo.ValidateDTOAGV(dto, out string validateDTOMsg))
//                {
//                    return BadRequest(ApiResponseFactory.Fail(null, validateDTOMsg));
//                }

                Project model = new Project();
                model.ID = dto.ID;
                model.ProjectCode = dto.ProjectCode;
                model.ProjectName = dto.ProjectName;
                model.ProjectStatus = dto.ProjectStatus ?? 0;
                model.Priotity = dto.Priotity;
                model.ActualDateEnd = dto.ActualDateEnd;
                model.ActualDateStart = dto.ActualDateStart;
                model.ProjectType = dto.ProjectType;
                model.BusinessFieldID = dto.BusinessFieldID;
                model.TypeProject = dto.TypeProject;
                model.ProjectManager = dto.ProjectManager;
                model.ContactID = dto.ContactID;
                model.Note = dto.Note;
                if (dto != null)
                {
                    if (dto.ID == 0)
                    {

                        string validateMsg;
                        //if (!_projectRepo.ValidateAGV(model, out validateMsg))
                        //{
                        //    return BadRequest(ApiResponseFactory.Fail(null, validateMsg));
                        //}
                        await _projectRepo.CreateAsync(model);
                    }
                    else
                    {

                        string validateMsg;
                        //if (!_projectRepo.ValidateAGV(model, out validateMsg))
                        //{
                        //    return BadRequest(ApiResponseFactory.Fail(null, validateMsg));
                        //}
                        await _projectRepo.UpdateAsync(model);
                    }
                }

//                if (dto.Tasks != null)
//                {

                    foreach (var item in dto.Tasks)
                    {
                        if (!_projectItemRepo.ValidateDTO(item, out string validateProjectItemMsg))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, validateProjectItemMsg));
                        }
                        ProjectItem projectItem = new ProjectItem();
                        projectItem.ID = item.ID;
                        projectItem.ProjectID = item.ID;
                        projectItem.Note = "";
                        projectItem.Mission = item.Mission;
                        projectItem.TypeProjectItem = item.TypeProjectItem;
                        projectItem.Status = item.Status;
                        projectItem.PercentItem = item.PercentItem;
                        projectItem.PlanStartDate = item.PlanStartDate;
                        projectItem.PlanEndDate = item.PlanEndDate;
                        projectItem.ActualEndDate = item.ActualEndDate;
                        projectItem.ActualStartDate = item.PlanStartDate;
                        projectItem.TotalDayPlan = (projectItem.PlanEndDate.Value.Date - projectItem.PlanStartDate.Value.Date).Days + 1;
                        projectItem.TotalDayActual = (projectItem.ActualEndDate.Value.Date - projectItem.PlanStartDate.Value.Date).Days + 1;
                        projectItem.CreatedDate = item.CreatedDate;
                        projectItem.UpdatedDate = item.UpdatedDate;
                        projectItem.IsDeleted = item.IsDeleted;
                        projectItem.UserID = item.UserID;
                        if (projectItem.ID == 0)
                        {
                            projectItem.STT = _projectItemRepo.GetMaxSTT(item.ProjectID);

//                            string validateMsg;
//                            //if (!_projectItemRepo.Validate(projectItem, out validateMsg))
//                            //{
//                            //    return BadRequest(ApiResponseFactory.Fail(null, validateMsg));
//                            //}
//                            _projectItemRepo.Create(projectItem);
//                        }
//                        else
//                        {


                            string validateMsg;
                            //if (!_projectItemRepo.Validate(projectItem, out validateMsg))
                            //{
                            //    return BadRequest(ApiResponseFactory.Fail(null, validateMsg));
                            //}
                            _projectItemRepo.Update(projectItem);
                        }
                    }
                }
                if (dto.Issues != null)
                {

                    foreach (var item in dto.Issues)
                    {
                        if (!_projectIssuesRepo.Validate(item, out var validateIssuesMessage))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, validateIssuesMessage));
                        }
                        ProjectIssues projectIssue = _projectIssuesRepo.MapFromProjectIssueDTOToFromProject(item);
                        if (projectIssue.ID == null || projectIssue.ID <= 0)
                        {
                            //if (!_projectIssuesRepo.Validate(item, out var error))
                            //{
                            //    return BadRequest(ApiResponseFactory.Fail(null, error));
                            //}
                            projectIssue.CreatedDate = DateTime.Now;
                            projectIssue.UpdatedDate = DateTime.Now;
                            _projectIssuesRepo.CreateAsync(projectIssue);

                        }
                        else
                        {
                            //if (!_projectIssuesRepo.Validate(item, out var error))
                            //{
                            //    return BadRequest(ApiResponseFactory.Fail(null, error));
                            //}
                            projectIssue.UpdatedDate = DateTime.Now;
                            _projectIssuesRepo.Update(projectIssue);
                        }
                    }
                    if (dto.Documents != null)
                    {

                        foreach (var item in dto.Documents)
                        {
                            if (!_projectDocumentsRepo.ValidateForSaveDTO(item, out string validateDocumentMsg))
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, validateDocumentMsg));
                            }
                            if (item.IsDeleted == true && item.ID > 0)
                            {
                                ProjectDocument deleteModel = _projectDocumentsRepo.GetByID(item.ID.Value);
                                if (deleteModel != null)
                                {
                                    deleteModel.IsDeleted = true;
                                    //  _projectDocumentsRepo.PrepareForSave(deleteModel, false);
                                    await _projectDocumentsRepo.UpdateAsync(deleteModel);
                                }
                                continue;
                            }

//                            int id = item.ID ?? 0;
//                            ProjectDocument model1 = id > 0 ? _projectDocumentsRepo.GetByID(id) : new ProjectDocument();
//                            model1.ProjectID = item.ProjectID ?? 0;
//                            model1.Name = item.Name;
//                            model1.Type = item.Type ?? 0;
//                            model1.FilePath = item.FilePath;
//                            model1.Version = item.Version;
//                            model1.Size = item.Size;
//                            model1.CreateBy = item.CreateBy;
//                            model1.UpdatedBy = item.UpdatedBy;
//                            model1.IsDeleted = false;

//                            //Validate sử dụng repo validation
//                            //if (!_projectDocumentsRepo.ValidateForSave(model1, out string message))
//                            //{
//                            //    return BadRequest(ApiResponseFactory.Fail(null, message));
//                            //}

                            if (id > 0)
                            {
                                // Sửa: ID > 0
                                model.ID = id;
                                //  _projectDocumentsRepo.PrepareForSave(model1, false); // isInsert = false
                                await _projectDocumentsRepo.UpdateAsync(model1);
                            }
                            else
                            {
                                // Thêm: ID = 0 hoặc null
                                //  _projectDocumentsRepo.PrepareForSave(model1, true); // isInsert = true
                                await _projectDocumentsRepo.CreateAsync(model1);
                            }
                        }
                    }
                }
                return Ok(ApiResponseFactory.Success(true, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("get-project-item")]
        public IActionResult GetProjectItem(ProjectItemRequestParam request)
        {
            try
            {
                var projectItem = SQLHelper<ProjectItemDTO>.ProcedureToListModel("spGetProjectItem",
                    new[] { "DateStart", "DateEnd", "@ProjectID", "@UserID", "@Keyword", "@Status" },
                    new object[] { request.DateStart, request.DateEnd, request.ProjectID, request.UserID, request.Keyword, request.Status });

                return Ok(ApiResponseFactory.Success(projectItem, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-project-item")]
        public async Task<IActionResult> SaveDataProjectItem([FromBody] List<ProjectItemDTO> projectItems)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                bool isTBP = _roleConfig.TBPEmployeeIds.Contains(currentUser.EmployeeID);
                bool isPBP = _roleConfig.PBPPositionCodes.Contains(currentUser.PositionCode);
                // 1) Hạng mục
                if (projectItems != null)
                {
                    foreach (var item in projectItems)
                    {
                        ProjectItem? existing = item.ID > 0 ? _projectItemRepo.GetByID(item.ID) : null;
                        int approved = existing?.IsApproved ?? 0;

                        // Xóa mềm
                        if (item.IsDeleted && item.ID != 0)
                        {
                            if (!(currentUser.IsAdmin || isTBP || isPBP))
                                return BadRequest(ApiResponseFactory.Fail(null, "Bạn không có quyền xóa hạng mục"));
                            if (approved > 0)
                                if (existing != null)
                                {
                                    existing.IsDeleted = true;
                                    await _projectItemRepo.UpdateAsync(existing);
                                }
                            continue;
                        }

                        if (!_projectItemRepo.ValidateDTO(item, out string validateProjectItemMsg))
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, validateProjectItemMsg));
                        }
                        ProjectItem projectItem = new ProjectItem();
                        projectItem.ID = item.ID;
                        projectItem.ProjectID = item.ProjectID;
                        projectItem.Note = "";
                        projectItem.Mission = item.Mission;
                        projectItem.TypeProjectItem = item.TypeProjectItem;
                        projectItem.Status = item.Status;
                        projectItem.PercentItem = item.PercentItem;
                        projectItem.PlanStartDate = item.PlanStartDate;
                        projectItem.PlanEndDate = item.PlanEndDate;
                        projectItem.ActualEndDate = item.ActualEndDate;
                        projectItem.ActualStartDate = item.PlanStartDate;
                        projectItem.TotalDayPlan = (projectItem.PlanEndDate.Value.Date - projectItem.PlanStartDate.Value.Date).Days + 1;
                        projectItem.TotalDayActual = (projectItem.ActualEndDate.Value.Date - projectItem.PlanStartDate.Value.Date).Days + 1;
                        //   projectItem.UserID = item.pic_employee_code;
                        projectItem.CreatedDate = item.CreatedDate;
                        projectItem.UpdatedDate = item.UpdatedDate;
                        projectItem.IsDeleted = item.IsDeleted;
                        projectItem.UserID = item.UserID;
                        if (projectItem.ID <= 0)
                        {
                            projectItem.STT = _projectItemRepo.GetMaxSTT(item.ProjectID);
                            string validateMsg;
                            if (!_projectItemRepo.ValidateAGV(projectItem, out validateMsg))
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, validateMsg));
                            }
                            _projectItemRepo.Create(projectItem);
                        }
                        else
                        {

                            string validateMsg;
                            //if (!_projectItemRepo.ValidateAGV(projectItem, out validateMsg))
                            //{
                            return BadRequest(ApiResponseFactory.Fail(null, validateMsg));
                        }
                        _projectItemRepo.Update(projectItem);
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






//    }
//}
