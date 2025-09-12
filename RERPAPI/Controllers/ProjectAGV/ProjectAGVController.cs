using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Project;
using RERPAPI.Model.DTO.ProjectAGV;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Model.Param.Project;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Project;

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
         private readonly ProjectIssuesRepo _projectIssuesRepo = new ProjectIssuesRepo();
        private readonly ProjectDocumentsRepo projectDocumentsRepo = new ProjectDocumentsRepo();

        [HttpPost("get-project-issues")]
        public async Task<ActionResult> spGetProjectIssues([FromBody] ProjectIssuesRequestParam request)
        {
            try
            {
                string procedureName = "spGetProjectIssues";
                string[] paramNames = new string[] { "@DateStart", "@DateEnd", "@ProjectID", "@DepartmentID", "@Keyword", "@EmployeeID" };
                object[] paramValues = new object[] { request.DateStart, request.DateEnd, request.ProjectID, request.DepartmentID, request.Keyword, request.EmployeeID };
                // 2. Gọi procedure thông qua helper
                var data = SQLHelper<ProjectIssueDTO>.ProcedureToList1(procedureName, paramNames, paramValues);

                // 3. Xử lý kết quả
                return Ok(ApiResponseFactory.Success(data, ""));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));

            }
        }
        [HttpPost("save-project-issues")]
        public async Task<IActionResult> SaveProjectIssues([FromBody] ProjectIssueDTO projectIssueDTO)
        {
            try
            {
                ProjectIssues projectIssue = _projectIssuesRepo.MapFromProjectIssueDTOToFromProject(projectIssueDTO);
                if (projectIssue.ID == null || projectIssue.ID <= 0)
                {
                    if (!_projectIssuesRepo.Validate(projectIssueDTO, out var error))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, error));
                    }
                    projectIssue.CreatedDate = DateTime.Now;
                    projectIssue.UpdatedDate = DateTime.Now;
                    _projectIssuesRepo.CreateAsync(projectIssue);
                    return Ok(new
                    {
                        data = projectIssue,
                        message = "Thêm thành công",
                        Status = 1
                    });
                }
                else
                {
                    if (!_projectIssuesRepo.Validate(projectIssueDTO, out var error))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, error));
                    }
                    projectIssue.UpdatedDate = DateTime.Now;
                    _projectIssuesRepo.Update(projectIssue);
                    return Ok(new
                    {
                        data = projectIssue,
                        message = "Sửa thành công",
                        Status = 1
                    });
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

                // Gọi store
                var ds = SQLHelper<ProjectDocumentDTO>.ProcedureToList1(
                    "spGetProjectDocument",
                    names,
                    values
                );

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

              
                foreach (ProjectDocumentDTO item in documents)
                {
                    // Xử lý xóa mềm trước
                    if (item.IsDeleted == true && item.ID > 0)
                    {
                        ProjectDocuments deleteModel = projectDocumentsRepo.GetByID(item.ID.Value);
                        if (deleteModel != null)
                        {
                            deleteModel.IsDeleted = true;
                            projectDocumentsRepo.PrepareForSave(deleteModel, false);
                            await projectDocumentsRepo.UpdateAsync(deleteModel);
                        }
                        continue; // Skip đến item tiếp theo
                    }

                    int id = item.ID ?? 0;
                    ProjectDocuments model = id > 0 ? projectDocumentsRepo.GetByID(id) : new ProjectDocuments();
                    model.ProjectID = item.ProjectID ?? 0;
                    model.Name = item.Name;
                    model.Type = item.Type ?? 0;
                    model.FilePath = item.FilePath;
                    model.Version = item.Version;
                    model.Size = item.Size;
                    model.CreateBy = item.CreateBy;
                    model.UpdatedBy = item.UpdatedBy;
                    model.IsDeleted = false;

                    // Validate sử dụng repo validation
                    if (!projectDocumentsRepo.ValidateForSave(model, out string message))
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, message));
                    }

                    if (id > 0)
                    {
                        // Sửa: ID > 0
                        model.ID = id;
                        projectDocumentsRepo.PrepareForSave(model, false); // isInsert = false
                        await projectDocumentsRepo.UpdateAsync(model);
                    }
                    else  
                    {
                        // Thêm: ID = 0 hoặc null
                        projectDocumentsRepo.PrepareForSave(model, true); // isInsert = true
                        await projectDocumentsRepo.CreateAsync(model);
                    }
                }

                return Ok(ApiResponseFactory.Success(true, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-projects")]
        //  [ApiKeyAuthorize]
        public async Task<IActionResult> GetProjects(int size, int page,
           DateTime dateTimeS, DateTime dateTimeE, string? projectType,
           int pmID, int leaderID, int bussinessFieldID, string? projectStatus,
           int customerID, int saleID, int userTechID, int globalUserID, string? keyword, bool isAGV = true)
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
                        int index = projectTypeIDs.IndexOf(Convert.ToInt32(item));
                        if (index >= 0 && index < typeCheck.Length)
                        {
                            typeCheck[index] = 1;
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(projectStatus))
                {
                    List<int> listStatus = _projectStatusRepo.GetAll().Select(x => x.ID).ToList();
                    projectStatus = string.Join(",", listStatus);
                }
                var project = SQLHelper<ProjectAGVResponseDTO>.ProcedureToList1("spGetProject",
                    new string[] {
                        "@PageSize", "@PageNumber", "@DateStart", "@DateEnd", "@FilterText", "@CustomerID", "@UserID",
                        "@ListProjectType", "@LeaderID", "@UserIDTech", "@EmployeeIDPM", "@1", "@2", "@3", "@4", "@5",
                        "@6", "@7", "@8", "@9", "@UserIDPriotity", "@BusinessFieldID", "@ProjectStatus"
                    },
                    new object[] {
                        size, page, dateTimeS, dateTimeE, keyword ?? "", customerID, saleID, projectType, leaderID,
                        userTechID, pmID, typeCheck[0] ,typeCheck[1] ,typeCheck[2] ,typeCheck[3] ,typeCheck[4] ,typeCheck[5]
                        ,typeCheck[6] ,typeCheck[7] ,typeCheck[8], globalUserID, bussinessFieldID, true
                    });
                foreach (var item in project)
                {

                    var lsttask = _projectItemRepo.GetAll(x => x.ProjectID == item.ID);
                    var tasks = lsttask.Select(x => new ProjectItemDTO
                    {
                        ID = x.ID,
                        ProjectID = x.ProjectID,
                        name = "",
                        Status = x.Status,
                        Mission = x.Mission,
                        TypeProjectItem = x.TypeProjectItem,
                        priority = 0,
                        PercentItem = x.PercentItem,
                        steps = "",
                        PlanStartDate = x.PlanStartDate,
                        PlanEndDate = x.PlanEndDate,
                        ActualEndDate = x.ActualEndDate,
                        pic_employee_code = "",
                        supports = "",
                        file_paths = "",
                        CreatedDate = x.CreatedDate,
                        UpdatedDate = x.UpdatedDate,
                           IsDeleted = x.IsDeleted,
                        UserID = x.UserID
                    }).ToList();
                    item.tasks = tasks;
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

                    item.issues = issues;
                    var lstDocument = projectDocumentsRepo.GetAll(d => d.ProjectID == item.ID && d.IsDeleted == false);


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

                    item.documents = documents;

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

                Project model = new Project();
                model.ID = dto.ID;
                model.ProjectCode = dto.ProjectCode;
                model.ProjectName = dto.ProjectName;
                model.ProjectStatus = dto.ProjectStatus;
                model.Priotity = dto.Priotity;
                model.ActualDateEnd = dto.ActualDateEnd;
                model.ActualDateStart = dto.ActualDateStart;
                model.ProjectType = dto.ProjectType;
                model.BusinessFieldID = dto.BusinessFieldID;
                model.TypeProject = dto.TypeProject;
                model.ProjectManager = dto.ProjectManager;
                model.ContactID = dto.ContactID;
                model.Note = dto.Note;
               
                if (dto.tasks != null)
                {

                    foreach (var item in dto.tasks)
                    {
                      
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

                            string validateMsg;
                            if (!_projectItemRepo.Validate(projectItem, out validateMsg))
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, validateMsg));
                            }
                            _projectItemRepo.Create(projectItem);
                        }
                        else
                        {
                            
                            _projectItemRepo.Create(projectItem);
                            string validateMsg;
                            if (!_projectItemRepo.Validate(projectItem, out validateMsg))
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, validateMsg));
                            }
                            _projectItemRepo.Update(projectItem);
                        }
                    }
                }

                if (dto.issues != null)
                {

                    foreach (var item in dto.issues)
                    {

                        ProjectIssues projectIssue = _projectIssuesRepo.MapFromProjectIssueDTOToFromProject(item);
                        if (projectIssue.ID == null || projectIssue.ID <= 0)
                        {
                            if (!_projectIssuesRepo.Validate(item, out var error))
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, error));
                            }
                            projectIssue.CreatedDate = DateTime.Now;
                            projectIssue.UpdatedDate = DateTime.Now;
                            _projectIssuesRepo.CreateAsync(projectIssue);
                            return Ok(new
                            {
                                data = projectIssue,
                                message = "Thêm thành công",
                                Status = 1
                            });
                        }
                        else
                        {
                            if (!_projectIssuesRepo.Validate(item, out var error))
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, error));
                            }
                            projectIssue.UpdatedDate = DateTime.Now;
                            _projectIssuesRepo.Update(projectIssue);
                            return Ok(new
                            {
                                data = projectIssue,
                                message = "Sửa thành công",
                                Status = 1
                            });
                        }
                    }
                    if (dto.documents != null)
                    {

                        foreach (var item in dto.documents)
                        {

                            // Xử lý xóa mềm trước
                            if (item.IsDeleted == true && item.ID > 0)
                            {
                                ProjectDocuments deleteModel = projectDocumentsRepo.GetByID(item.ID.Value);
                                if (deleteModel != null)
                                {
                                    deleteModel.IsDeleted = true;
                                    projectDocumentsRepo.PrepareForSave(deleteModel, false);
                                    await projectDocumentsRepo.UpdateAsync(deleteModel);
                                }
                                continue; // Skip đến item tiếp theo
                            }

                            int id = item.ID ?? 0;
                            ProjectDocuments model1 = id > 0 ? projectDocumentsRepo.GetByID(id) : new ProjectDocuments();
                            model1.ProjectID = item.ProjectID ?? 0;
                            model1.Name = item.Name;
                            model1.Type = item.Type ?? 0;
                            model1.FilePath = item.FilePath;
                            model1.Version = item.Version;
                            model1.Size = item.Size;
                            model1.CreateBy = item.CreateBy;
                            model1.UpdatedBy = item.UpdatedBy;
                            model1.IsDeleted = false;

                            // Validate sử dụng repo validation
                            if (!projectDocumentsRepo.ValidateForSave(model1, out string message))
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, message));
                            }

                            if (id > 0)
                            {
                                // Sửa: ID > 0
                                model.ID = id;
                                projectDocumentsRepo.PrepareForSave(model1, false); // isInsert = false
                                await projectDocumentsRepo.UpdateAsync(model1);
                            }
                            else
                            {
                                // Thêm: ID = 0 hoặc null
                                projectDocumentsRepo.PrepareForSave(model1, true); // isInsert = true
                                await projectDocumentsRepo.CreateAsync(model1);
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
                var projectItem = SQLHelper<ProjectItemDTO>.ProcedureToList1("spGetProjectItem",
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
                bool isTBP = currentUser.EmployeeID == 54;
                bool isPBP = currentUser.PositionCode == "CV57" || currentUser.PositionCode == "CV28";
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
                                return BadRequest(ApiResponseFactory.Fail(null, "Hạng mục đã duyệt không thể xóa"));

                            if (existing != null)
                            {
                                existing.IsDeleted = true;
                                _projectItemRepo.Update(existing);
                            }
                            continue;
                        }
                        //    string category = " Của Hạng mục công việc";
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
                            if (!_projectItemRepo.ValidateAGV(projectItem, out validateMsg))
                            {
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
       
     

      

      
    }
}
