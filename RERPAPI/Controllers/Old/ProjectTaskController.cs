using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NPOI.HSSF.Record.Chart;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.Style.XmlAccess;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.Project.Procedure;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Project;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ZXing;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using ProjectTaskApprove = RERPAPI.Model.Entities.ProjectTaskApprove;
using ProjectTaskLog = RERPAPI.Model.Entities.ProjectTaskLog;
using UserTeam = RERPAPI.Model.Entities.UserTeam;
namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTaskController : ControllerBase
    {
        ProjectTaskRepo _projectTaskRepo;
        ProjectTaskGroupRepo _groupRepo;
        ProjectTaskChecklistRepo _checklistRepo;
        ProjectTaskAttachmentRepo _attachmentRepo;
        ProjectTaskEmployeeRepo _projectTaskEmployeeRepo;
        ProjectRepo _projectRepo;
        EmployeeRepo _employeeRepo;
        SendEmailReceiveProjectTaskClass _sendEmailService;

        ProjectTaskApproveRepo _projectTaskApproveRepo;

        ProjectTaskLogRepo _projectTaskLogRepo;
        ProjectTaskTypeRepo _projectTaskTypeRepo;
        ProjectTaskAdditionalRepo _projectTaskAdditionalRepo;
        DepartmentRepo _departmentRepo;
        ProjectItemRepo _projectItemRepo;
        public ProjectTaskController(ProjectTaskRepo projectTaskRepo,
            ProjectTaskGroupRepo groupRepo,
            ProjectTaskChecklistRepo checklistRepo,
            ProjectTaskAttachmentRepo attachmentRepo,
            ProjectTaskEmployeeRepo projectTaskEmployeeRepo,
            ProjectRepo projectRepo,

            ProjectTaskApproveRepo projectTaskApproveRepo,
            ProjectTaskLogRepo projectTaskLogRepo,
            EmployeeRepo employeeRepo,
            SendEmailReceiveProjectTaskClass sendEmailService,
            ProjectTaskTypeRepo projectTaskTypeRepo,
            ProjectTaskAdditionalRepo projectTaskAdditionalRepo,
            DepartmentRepo departmentRepo
            ,

            ProjectItemRepo projectItemRepo
            )
        {
            _projectTaskRepo = projectTaskRepo;
            _groupRepo = groupRepo;
            _checklistRepo = checklistRepo;
            _attachmentRepo = attachmentRepo;
            _projectTaskEmployeeRepo = projectTaskEmployeeRepo;
            _projectRepo = projectRepo;

            _projectTaskApproveRepo = projectTaskApproveRepo;

            _projectTaskLogRepo = projectTaskLogRepo;
            _employeeRepo = employeeRepo;
            _sendEmailService = sendEmailService;
            _projectTaskTypeRepo = projectTaskTypeRepo;
            _projectTaskAdditionalRepo = projectTaskAdditionalRepo;
            _departmentRepo = departmentRepo;
            _projectItemRepo = projectItemRepo;
        }

        public class MoveRequest
        {
            public int ProjectTaskGroupID { get; set; }
            public int OrderIndex { get; set; }
        }

        //[HttpGet]
        //public IActionResult Get([FromQuery(Name = "projectId")] int projectID)
        //{
        //    try
        //    {
        //        List<spGetProjectTask> projectTasks = SQLHelper<spGetProjectTask>.ProcedureToListModel("spGetProjectTask", new string[] { "@ProjectID" }, new object[] { projectID });
        //        return Ok(ApiResponseFactory.Success(projectTasks));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, "Failed to retrieve project tasks."));
        //    }
        //}


        [HttpGet]
        public async Task<IActionResult> GetProjectTask(DateTime dateStart, DateTime dateEnd, int status)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                dateStart = dateStart.Date;
                dateEnd = dateEnd.Date.AddDays(1).AddSeconds(-1);
                var param = new
                {
                    EmployeeID = currentUser.EmployeeID,
                    // EmployeeID = 610,
                    DateStart = dateStart,
                    DateEnd = dateEnd,
                    Status = status
                };
                var projectTasks = await SqlDapper<spGetProjectTaskByEmployeeID>.ProcedureToListTAsync("spGetProjectTaskByEmployeeID", param);
                //var projectTasks = SQLHelper<spGetProjectTaskByEmployeeID>.ProcedureToListModel("spGetProjectTaskByEmployeeID", new string[] { "@EmployeeID" }, new object[] { 91 });
                return Ok(ApiResponseFactory.Success(new
                {
                    ProjectTask = projectTasks.OrderByDescending(x => x.UpdatedDate),
                    UserID = currentUser.EmployeeID
                    //UserID = 91
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to retrieve project tasks."));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var projectTask = await _projectItemRepo.GetByIDAsync(id);
                if (projectTask == null)
                {
                    return NotFound(ApiResponseFactory.Fail(null, "Project task not found."));
                }
                return Ok(ApiResponseFactory.Success(projectTask));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to retrieve project task."));
            }
        }


        [HttpGet("list-project-task")]
        public async Task<IActionResult> GetProjectTaskForList(int projectID = 0, bool isPersionalProject = false)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var param = new
                {
                    p_ProjectID = projectID,
                    p_IsPersonalProject = isPersionalProject,
                    p_EmployeeID = currentUser.EmployeeID,
                };
                var result = await SqlDapper<object>.ProcedureToListAsync("spGetListProjectTask", param);
                return Ok(ApiResponseFactory.Success(result, null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to retrieve project tasks."));
            }
        }

        [HttpGet("get_project_task_tree")]
        public async Task<IActionResult> GetProjectTaskTree(
            [FromQuery] DateTime dateStart,
            [FromQuery] DateTime dateEnd,
            [FromQuery] int projectID = 0,
            [FromQuery] string keyword = ""
            )
        {
            try
            {

                var param = new
                {
                    DateStart = dateStart,
                    DateEnd = dateEnd,
                    ProjectID = projectID == -1 ? 0 : projectID,
                    Keyword = keyword
                };
                //var data = SQLHelper<spGetProjectTaskTreeParam>.ProcedureToListModel("spGetProjectTaskTree", new string[] { "@DateStart", "@DateEnd", "@ProjectID", "@Keyword" }, new object[] { dateStart, dateEnd, projectID, keyword });
                var data = await SqlDapper<spGetProjectTaskTreeParam>.ProcedureToListTAsync("spGetProjectTaskTree", param);

                var result = _projectTaskRepo.BuldTreeProjectTask(data);

                return Ok(ApiResponseFactory.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to get project task tree."));
            }

        }

        // -- Get project task time line (dùng cho lấy dữ liệu hiển thị ở timeline) ---
        [HttpGet("project-task-timeline")]
        public async Task<IActionResult> GetProjectTaskTimeLine(DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                dateStart = dateStart.Date;
                dateEnd = dateEnd.Date.AddDays(1).AddSeconds(-1);
                var param = new
                {
                    EmployeeID = currentUser.EmployeeID,
                    DateStart = dateStart,
                    DateEnd = dateEnd
                };
                var projectTasks = await SqlDapper<object>.ProcedureToListAsync("spGetProjectTaskTimeLine", param);
                return Ok(ApiResponseFactory.Success(projectTasks));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to get project tasks time line."));
            }
        }

        // -- Get project task view status (Dùng cho lấy dữ liệu hiển thị ở project task view status tổng) ---
        [HttpGet("project-task-view-status")]
        public async Task<IActionResult> GetProjectTaskViewStatus(
            [FromQuery] DateTime dateStart,
            [FromQuery] DateTime dateEnd,
            [FromQuery] int departmentID = 0,
            [FromQuery] int teamID = 0,
            [FromQuery] int userID = 0,
            [FromQuery] int projectID = 0,
            [FromQuery] string keyword = ""
            )
        {
            try
            {
                //var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                //var currentUser = ObjectMapper.GetCurrentUser(claims);
                dateStart = dateStart.Date;
                dateEnd = dateEnd.Date.AddDays(1).AddTicks(-1);
                var param = new
                {
                    DateStart = dateStart,
                    DateEnd = dateEnd,
                    DepartmentID = departmentID,
                    TeamID = teamID,
                    UserID = userID,
                    ProjectID = projectID,
                    Keyword = keyword
                };
                var projectTasks = await SqlDapper<object>.ProcedureToListAsync("spGetSummarizeProjectTask", param);
                return Ok(ApiResponseFactory.Success(projectTasks));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to get project tasks view status."));
            }
        }


        [HttpGet("project-task-type")]
        public IActionResult GetProjectTaskType([FromQuery] List<int> listEmployeeAsignee)
        {
            try
            {
                //var listEmployee = _employeeRepo.GetAll(x => listEmployeeAsignee.Contains(x.ID));
                var listEmployee = _employeeRepo.GetAll(x => listEmployeeAsignee.Contains(x.ID));

                var listDepartmentIDs = listEmployee.Select(x => x.DepartmentID).Distinct();

                var result = _projectTaskTypeRepo.GetAll().Where(x => x.IsDeleted == false && (listDepartmentIDs.Contains(x.DepartmentID) || x.DepartmentID == null || x.DepartmentID == 0)).Distinct().ToList();
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to get project task type."));
            }
        }

        // -- Get all project ---

        [HttpGet("get-all-project")]
        public IActionResult getAllProject()
        {
            try
            {
                var result = _projectRepo.GetAll(x => x.IsDeleted == false).OrderByDescending(x => x.ID);
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        #region check list 
        // --- Checklists ---
        [HttpGet("{taskId}/Checklists")]
        public IActionResult GetChecklists(int taskId)
        {
            try
            {
                var list = _checklistRepo.GetAll(x => x.ProjectTaskID == taskId && (x.IsDeleted == null || x.IsDeleted == false)).OrderBy(x => x.OrderIndex);
                return Ok(ApiResponseFactory.Success(list));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to retrieve checklists."));
            }
        }

        [HttpPost("Checklists")]
        public async Task<IActionResult> AddChecklist([FromBody] ProjectTaskChecklist item)
        {
            try
            {
                item.CreatedDate = DateTime.Now;
                await _checklistRepo.CreateAsync(item);
                return Ok(ApiResponseFactory.Success(item));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to create checklist item."));
            }
        }

        [HttpDelete("Checklists/{id}")]
        public async Task<IActionResult> DeleteChecklist(int id)
        {
            try
            {
                var exitCheckList = await _checklistRepo.GetByIDAsync(id);
                exitCheckList.IsDeleted = true;
                if (await _checklistRepo.UpdateAsync(exitCheckList) <= 0)
                {
                    BadRequest(ApiResponseFactory.Fail(null, "Checklist delete fail."));
                }
                return Ok(ApiResponseFactory.Success(exitCheckList));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to delete checklist item."));
            }
        }

        [HttpPut("Checklists/{id}")]
        public async Task<IActionResult> UpdateChecklist(int id, [FromBody] ProjectTaskChecklist item)
        {
            try
            {
                var existing = await _checklistRepo.GetByIDAsync(id);
                if (existing == null) return NotFound(ApiResponseFactory.Fail(null, "Checklist item not found."));

                existing.ChecklistTitle = item.ChecklistTitle ?? existing.ChecklistTitle;
                existing.IsDone = item.IsDone ?? existing.IsDone;
                await _checklistRepo.UpdateAsync(existing);
                return Ok(ApiResponseFactory.Success(existing));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to update checklist item."));
            }
        }

        #endregion

        #region Project Task Additional Information (Thông tin bổ sung cho công việc)
        // -- Project Task Additional Information ---
        [HttpGet("{taskId}/Additional")]
        public IActionResult GetAdditional(int taskId)
        {
            try
            {
                var list = _projectTaskAdditionalRepo.GetAll(x => x.ProjectTaskID == taskId && (x.IsDeleted == null || x.IsDeleted == false)).OrderBy(x => x.ID);
                return Ok(ApiResponseFactory.Success(list));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to retrieve checklists."));
            }
        }

        [HttpPost("Additional")]
        public async Task<IActionResult> AddAdditional([FromBody] ProjectTaskAdditional item)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (item.Description == null || item.Description.Trim().Count() <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Hãy nhập nội dung phát sinh."));
                }

                if (item.ID <= 0)
                {
                    item.ID = 0;
                    await _projectTaskAdditionalRepo.CreateAsync(item);

                    var newProjectTaskLog = new ProjectTaskLog
                    {
                        ProjectTaskID = item.ProjectTaskID,
                        TypeLog = "Phát sinh",
                        ContentLog = $"- {currentUser.Name} tạo phát sinh với nội dung: {item.Description}"
                    };
                    await _projectTaskLogRepo.CreateAsync(newProjectTaskLog);
                    return Ok(ApiResponseFactory.Success(item));
                }
                else
                {
                    var exitAdditional = _projectTaskAdditionalRepo.GetByID(item.ID);
                    exitAdditional.Description = item.Description;
                    exitAdditional.IsDeleted = item.IsDeleted;

                    if (item.IsDeleted ?? false && exitAdditional.IsDeleted != item.IsDeleted)
                    {
                        var newProjectTaskLog = new ProjectTaskLog
                        {
                            ProjectTaskID = item.ProjectTaskID,
                            TypeLog = "Xóa phát sinh",
                            ContentLog = $"- {currentUser.Name} xóa phát sinh có nội dung: {exitAdditional.Description}"
                        };
                        await _projectTaskLogRepo.UpdateAsync(newProjectTaskLog);
                    }
                    else if (exitAdditional.Description.Trim() != item.Description.Trim())
                    {
                        var newProjectTaskLog = new ProjectTaskLog
                        {
                            ProjectTaskID = item.ProjectTaskID,
                            TypeLog = "Sửa phát sinh",
                            ContentLog = $"- {currentUser.Name} sửa nội dung phát sinh từ: {exitAdditional.Description} thành: {item.Description}"
                        };
                        await _projectTaskLogRepo.UpdateAsync(newProjectTaskLog);
                    }
                    await _projectTaskAdditionalRepo.UpdateAsync(item);

                    return Ok(ApiResponseFactory.Success(item));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to create checklist item."));
            }
        }
        #endregion

        // --- Attachments ---
        [HttpGet("Attachments/{taskId}")]
        public IActionResult GetAttachments(int taskId)
        {
            try
            {
                var list = _attachmentRepo.GetAll(x => x.ProjectTaskID == taskId && (!x.IsDeleted ?? true));
                return Ok(ApiResponseFactory.Success(list));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to retrieve attachments."));
            }
        }

        [HttpPut("Files")]
        public async Task<IActionResult> ChangeFiles([FromBody] ProjectTaskAttachment item)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (item.ID <= 0)
                {
                    var newProjectTaskAttachment = new ProjectTaskAttachment
                    {
                        FileName = item.FileName,
                        FilePath = item.FilePath,
                        EmployeeUploadID = currentUser.ID,
                        UploadedDate = DateTime.Now,
                        ProjectTaskID = item.ProjectTaskID,
                        Type = 1, // File
                    };
                    if (await _attachmentRepo.CreateAsync(newProjectTaskAttachment) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to upload file."));
                    }
                    return Ok(ApiResponseFactory.Success(newProjectTaskAttachment));
                }
                else
                {
                    var exitFile = await _attachmentRepo.GetByIDAsync(item.ID);
                    if (exitFile == null)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to change file."));
                    }
                    exitFile.IsDeleted = item.IsDeleted ?? exitFile.IsDeleted;
                    exitFile.FileName = item.FileName ?? exitFile.FileName;
                    if (await _attachmentRepo.UpdateAsync(exitFile) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to change file."));
                    }
                    return Ok(ApiResponseFactory.Success(exitFile));
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to upload file."));
            }
        }


        [HttpPut("Links")]
        public async Task<IActionResult> AddLinks([FromBody] ProjectTaskAttachment item)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (item.ID <= 0)
                {
                    var newProjectTaskAttachment = new ProjectTaskAttachment
                    {
                        FileName = item.FileName,
                        FilePath = item.FilePath,
                        EmployeeUploadID = currentUser.ID,
                        UploadedDate = DateTime.Now,
                        ProjectTaskID = item.ProjectTaskID,
                        Type = 2, // Link
                    };
                    if (await _attachmentRepo.CreateAsync(newProjectTaskAttachment) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to upload file."));
                    }
                    return Ok(ApiResponseFactory.Success(newProjectTaskAttachment));
                }
                else
                {
                    var exitLink = await _attachmentRepo.GetByIDAsync(item.ID);
                    if (exitLink == null)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to change link."));
                    }
                    exitLink.IsDeleted = item.IsDeleted ?? exitLink.IsDeleted;
                    exitLink.FileName = item.FileName ?? exitLink.FileName;
                    exitLink.FilePath = item.FilePath ?? exitLink.FilePath;
                    if (await _attachmentRepo.UpdateAsync(exitLink) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to change link."));
                    }
                    return Ok(ApiResponseFactory.Success(exitLink));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to upload file."));
            }
        }

        // --- Project Task Employee ---

        // -- Update or Add employee to project task (When user check for chose user, tick or untick (assignee or related) or user change assignee front end will call this api) ---
        [HttpPut("employee")]
        public async Task<IActionResult> UpdateEmployees(int projectTaskID
            , int employeeType // 1: assignee , 2: related
            , bool isDeleted // true: remove employee , false: add or keep employee 
            , int employeeID)
        {
            try
            {
                var existing = _projectTaskEmployeeRepo.GetAll(x => x.ProjectTaskID == projectTaskID && x.Type == employeeType && x.EmployeeID == employeeID).FirstOrDefault();

                if (existing == null)
                {
                    var item = new ProjectTaskEmployee
                    {
                        ProjectTaskID = projectTaskID,
                        Type = employeeType,
                        EmployeeID = employeeID,
                        IsDeleted = false
                    };
                    await _projectTaskEmployeeRepo.CreateAsync(item);
                    if (employeeType == 1)
                    {
                        var projectTask = await _projectTaskRepo.GetByIDAsync(projectTaskID);
                        var employeeValue = await _employeeRepo.GetByIDAsync(projectTask.EmployeeID ?? 0);
                        SendEmailReceiveProjectTaskParam sendEmailContent = new SendEmailReceiveProjectTaskParam
                        {
                            projectTaskName = projectTask.Title,
                            projectTaskCode = projectTask.Code,
                            nameAsigner = employeeValue.FullName,
                            startDate = projectTask.PlanStartDate?.ToString("dd/MM/yyyy") ?? "",
                            endDate = projectTask.PlanEndDate?.ToString("dd/MM/yyyy") ?? "",
                            discription = projectTask.Description,
                        };
                        await _sendEmailService.SendEmailReceiveProjectTask(sendEmailContent);
                    }
                }
                else
                {
                    existing.IsDeleted = isDeleted;
                    await _projectTaskEmployeeRepo.UpdateAsync(existing);
                }
                return Ok(ApiResponseFactory.Success(existing));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to update project task employee."));
            }
        }

        [HttpGet("employee/{taskId}")]
        public async Task<IActionResult> GetAllEmployeeByTaskId(int taskId
            , int typeEmployee // 1: Người thực hiện , 2: Người liên quan 
            )
        {
            try
            {
                if (taskId == null || taskId <= 0 || typeEmployee <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Failed to get employee task."));
                }
                var lstEmployeeTask = _projectTaskEmployeeRepo.GetAll(x => x.ProjectTaskID == taskId && (!x.IsDeleted ?? true) && (x.Type == typeEmployee));

                return Ok(ApiResponseFactory.Success(lstEmployeeTask));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to get employee task."));
            }
        }

        // Lấy log theo projectTaskID

        [HttpGet("project-task-log/{projectTaskID}")]
        public async Task<IActionResult> GetProjectTaskLogByProjectTaskID(int projectTaskID)
        {
            try
            {
                if (projectTaskID == null || projectTaskID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Failed to get project task log."));
                }

                var projectTaskLog = _projectTaskLogRepo.GetAll(x => x.ProjectTaskID == projectTaskID);
                return Ok(ApiResponseFactory.Success(projectTaskLog));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to get project task log."));
            }
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveData([FromBody] ProjectTaskParam projectTask)
        {
            try
            {
                if (projectTask == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Project task data is null."));
                }

                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                if (currentUser.EmployeeID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Invalid user."));
                }
                if (projectTask.ID > 0)
                {
                    var existingTask = await _projectItemRepo.GetByIDAsync(projectTask.ID);
                    if (existingTask == null || existingTask.ID <= 0)
                    {
                        return NotFound(ApiResponseFactory.Fail(null, "Project task not found."));
                    }


                    if (projectTask.Status == 3 && projectTask.ActualEndDate.HasValue
                        && projectTask.ActualStartDate.HasValue && projectTask.PlanEndDate.HasValue
                        && projectTask.PlanStartDate.HasValue && ((existingTask.PlanStartDate?.Date != projectTask.PlanStartDate?.Date)
                        || (existingTask.ActualStartDate?.Date != projectTask.ActualStartDate?.Date) || (existingTask.PlanEndDate?.Date != projectTask.PlanEndDate?.Date)
                        || (existingTask.ActualEndDate?.Date != projectTask.ActualEndDate?.Date)))
                    {
                        var dayActual = (projectTask.ActualEndDate.Value.Date - projectTask.ActualStartDate.Value.Date).TotalDays + 1;
                        var dayPlan = (projectTask.PlanEndDate.Value.Date - projectTask.PlanStartDate.Value.Date).TotalDays + 1;
                        if ((dayActual - dayPlan) > 0)
                        {
                            existingTask.PercentOverTime = Math.Round((decimal)(dayActual - dayPlan) / (decimal)dayPlan, 2);
                        }
                        else
                        {
                            existingTask.PercentOverTime = 0;
                        }
                    }
                    var newProjectTaskLog = new ProjectTaskLog
                    {
                        ProjectTaskID = existingTask.ID,
                        TypeLog = "Thay đổi thời gian",
                        ContentLog = ""
                    };
                    // variable dùng để kiểm tra xem có thời gian nào thay đổi hay không. 
                    int changeTime = 0;


                    if (projectTask.PlanStartDate != null && existingTask.PlanStartDate?.Date != projectTask.PlanStartDate?.Date)
                    {

                        newProjectTaskLog.ContentLog += $"- {currentUser.FullName} đã thay đổi ngày bắt đầu dự kiến từ {existingTask.PlanStartDate?.ToString("dd/MM/yyyy")} thành {projectTask.PlanStartDate?.ToString("dd/MM/yyyy")}. \\n";

                        changeTime++;
                        existingTask.PlanStartDate = projectTask.PlanStartDate?.Date;
                    }

                    if (projectTask.PlanEndDate != null && existingTask.PlanEndDate?.Date != projectTask.PlanEndDate?.Date)
                    {
                        newProjectTaskLog.ContentLog += $"- {currentUser.FullName} đã thay đổi ngày kết thúc dự kiến từ {existingTask.PlanEndDate?.ToString("dd/MM/yyyy")} thành {projectTask.PlanEndDate?.ToString("dd/MM/yyyy")}. \\n";
                        changeTime++;
                        existingTask.PlanEndDate = projectTask.PlanEndDate?.Date;

                    }



                    if (projectTask.ActualStartDate != null && existingTask.ActualStartDate?.Date != projectTask.ActualStartDate?.Date)
                    {
                        newProjectTaskLog.ContentLog += $"- {currentUser.FullName} đã thay đổi ngày bắt đầu thực tế từ {existingTask.ActualStartDate?.ToString("dd/MM/yyyy")} thành {projectTask.ActualStartDate?.ToString("dd/MM/yyyy")}. \\n";
                        changeTime++;
                        existingTask.ActualStartDate = projectTask.ActualStartDate?.Date;

                    }
                    else if (projectTask.ActualStartDate == null && existingTask.ActualStartDate?.Date != projectTask.ActualStartDate?.Date)
                    {
                        newProjectTaskLog.ContentLog += $"- {currentUser.FullName} đã xóa ngày bắt đầu thực tế. \\n";
                        changeTime++;
                        existingTask.ActualStartDate = projectTask.ActualStartDate?.Date;
                    }

                    if (projectTask.ActualEndDate != null && existingTask.ActualEndDate?.Date != projectTask.ActualEndDate?.Date)
                    {
                        if (existingTask.ActualEndDate == null)
                        {
                            newProjectTaskLog.ContentLog += $"- {currentUser.FullName} đã thay đổi ngày kết thúc thực tế thành {projectTask.ActualEndDate?.ToString("dd/MM/yyyy")}. \\n";
                        }
                        else
                        {
                            newProjectTaskLog.ContentLog += $"- {currentUser.FullName} đã thay đổi ngày kết thúc thực tế từ {existingTask.ActualEndDate?.ToString("dd/MM/yyyy")} thành {projectTask.ActualEndDate?.ToString("dd/MM/yyyy")}. \\n";

                        }
                        changeTime++;
                        existingTask.ActualEndDate = projectTask.ActualEndDate?.Date;
                        projectTask.Status = 3;

                    }
                    else if (projectTask.ActualEndDate == null && existingTask.ActualEndDate?.Date != projectTask.ActualEndDate?.Date)
                    {
                        newProjectTaskLog.ContentLog += $"- {currentUser.FullName} đã xóa ngày kết thúc thực tế. \\n";
                        changeTime++;
                        existingTask.ActualEndDate = projectTask.ActualEndDate?.Date;
                        projectTask.Status = 2;

                    }

                    if (changeTime > 0)
                    {
                        await _projectTaskLogRepo.CreateAsync(newProjectTaskLog);
                    }



                    var listEmail1 = new List<String>();

                    if (projectTask.Employee != null && projectTask.Employee.Count > 0)
                    {
                        foreach (var emp in projectTask.Employee)
                        {
                            var employeeValue = await _employeeRepo.GetByIDAsync(emp);
                            if (emp != currentUser.EmployeeID)
                            {
                                listEmail1.Add(employeeValue.EmailCongTy);
                            }
                        }
                    }
                    var employeeAsigner = await _employeeRepo.GetByIDAsync(existingTask.EmployeeIDRequest ?? 0);

                    listEmail1.Add(employeeAsigner.EmailCongTy);

                    List<UserTeam> leaders1 = new List<UserTeam>();
                    if (projectTask.IsPersonalProject != true)
                    {
                        string usersString = "";

                        if (projectTask.Employee.Count > 0)
                        {
                            usersString = string.Join(",", projectTask.Employee);
                        }

                        var param = new
                        {
                            p_UserID = usersString
                        };
                        leaders1 = await SqlDapper<UserTeam>.ProcedureToListTAsync("spGetLeaderTeam", param);
                    }


                    if (projectTask.EmployeeRelate != null && projectTask.EmployeeRelate.Count > 0)
                    {
                        if (!projectTask.EmployeeRelate.Contains(currentUser.EmployeeID) && !projectTask.Employee.Contains(currentUser.EmployeeID) && projectTask.EmployeeIDRequest != currentUser.EmployeeID)
                        {
                            projectTask.EmployeeRelate.Add(currentUser.EmployeeID);
                        }


                        if (leaders1.Count > 0)
                        {
                            foreach (var item in leaders1)
                            {
                                if (item.LeaderID != null && !projectTask.EmployeeRelate.Contains(item.LeaderID ?? -1))
                                {
                                    projectTask.EmployeeRelate.Add(item.LeaderID ?? 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!projectTask.Employee.Contains(currentUser.EmployeeID) && projectTask.EmployeeIDRequest != currentUser.EmployeeID)
                        {
                            projectTask.EmployeeRelate = new List<int>();
                            projectTask.EmployeeRelate.Add(currentUser.EmployeeID);
                        }

                        if (leaders1.Count > 0)
                        {
                            foreach (var item in leaders1)
                            {
                                if (item.LeaderID != null && !projectTask.EmployeeRelate.Contains(item.LeaderID ?? -1))
                                {
                                    projectTask.EmployeeRelate.Add(item.LeaderID ?? 0);
                                }
                            }
                        }
                    }
                    var listEmployeeRelate = new List<string>();
                    foreach (var emp in projectTask.EmployeeRelate)
                    {
                        var employeeValue = await _employeeRepo.GetByIDAsync(emp);
                        if (employeeValue != null)
                        {
                            listEmployeeRelate.Add(employeeValue.EmailCongTy);
                        }
                    }


                    string listEmployeeRelateString1 = string.Join(",", listEmployeeRelate.Distinct());
                    string listToEmailString1 = string.Join(",", listEmail1.Distinct());
                    var employeeAsigner1 = await _employeeRepo.GetByIDAsync(existingTask.EmployeeIDRequest ?? 0);


                    if (projectTask.Status == 3 && existingTask.Status != projectTask.Status && listEmployeeRelateString1.Count() > 0)
                    {
                        SendEmailReceiveProjectTaskParam sendEmailContent1 = new SendEmailReceiveProjectTaskParam
                        {
                            emailRecive = listToEmailString1,
                            emailCC = listEmployeeRelateString1,
                            projectTaskName = existingTask.Mission,
                            projectTaskCode = existingTask.Code,
                            nameAsigner = employeeAsigner1.FullName,
                            startDate = existingTask.PlanStartDate?.ToString("dd/MM/yyyy") ?? "",
                            endDate = existingTask.PlanEndDate?.ToString("dd/MM/yyyy") ?? "",
                            completedDate = DateTime.Now.ToString("dd/MM/yyyy"),
                        };
                        _ = Task.Run(() => _sendEmailService.SendEmailCompleteProjectTask(sendEmailContent1));

                    }
                    existingTask.Status = projectTask.Status;


                    existingTask.Mission = projectTask.Mission;
                    existingTask.Description = projectTask.Description;
                    existingTask.ProjectID = projectTask.ProjectID;
                    existingTask.EmployeeIDRequest = projectTask.EmployeeIDRequest;
                    existingTask.IsPersonalProject = projectTask.IsPersonalProject;
                    existingTask.IsAdditional = projectTask.IsAdditional;
                    existingTask.TaskComplexity = projectTask.TaskComplexity;
                    existingTask.ParentID = projectTask.ParentID;
                    existingTask.TypeProjectItem = projectTask.TypeProjectItem;

                    if (existingTask.IsApproved == null || existingTask.IsApproved <= 1)
                    {
                        if (existingTask.EmployeeIDRequest > 0)
                        {
                            existingTask.IsApproved = 1; // Chờ duyệt,
                        }
                        else
                        {
                            existingTask.IsApproved = 0; // chưa duyệt
                        }
                    }
                    existingTask.UpdatedDate = DateTime.Now;
                    if (await _projectItemRepo.UpdateAsync(existingTask) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to update project task."));
                    }
                    return Ok(ApiResponseFactory.Success(existingTask));
                }
                else
                {
                    var newProjectTask = new ProjectItem
                    {
                        ProjectID = projectTask.ProjectID,
                        Mission = projectTask.Mission,
                        Description = projectTask.Description,
                        ActualStartDate = projectTask.ActualStartDate,
                        ActualEndDate = projectTask.ActualEndDate,
                        Status = projectTask.Status,
                        IsDeleted = false,
                        EmployeeIDRequest = projectTask.EmployeeIDRequest,
                        EmployeeCreateID = currentUser.EmployeeID,
                        PlanStartDate = projectTask.PlanStartDate,
                        PlanEndDate = projectTask.PlanEndDate,
                        IsPersonalProject = projectTask.IsPersonalProject,
                        TypeProjectItem = projectTask.TypeProjectItem ?? 1,
                        IsAdditional = projectTask.IsAdditional,
                        TaskComplexity = projectTask.TaskComplexity,
                        ParentID = projectTask.ParentID

                    };

                    var employeeCreate = _employeeRepo.GetByID(currentUser.EmployeeID);


                    if (projectTask.Status == 3 && projectTask.ActualEndDate != null
                        && projectTask.ActualStartDate != null && projectTask.PlanEndDate != null
                        && projectTask.PlanStartDate != null)
                    {
                        var dayActual = (projectTask.ActualEndDate.Value.Date - projectTask.ActualStartDate.Value.Date).TotalDays + 1;
                        var dayPlan = (projectTask.PlanEndDate.Value.Date - projectTask.PlanStartDate.Value.Date).TotalDays + 1;
                        if ((dayActual - dayPlan) > 0)
                        {
                            newProjectTask.PercentOverTime = Math.Round((decimal)(dayActual - dayPlan) / (decimal)dayPlan, 2);
                        }
                        else
                        {
                            newProjectTask.PercentOverTime = 0;
                        }
                    }

                     if (projectTask.ProjectID != null && projectTask.ProjectID > 0)
                    {
                        newProjectTask.Code = _projectItemRepo.GenerateProjectItemCode(projectTask.ProjectID ?? 0).Trim();

                    }
                    else
                    {
                        //var employeeById = _employeeRepo.GetByID(currentUser.EmployeeID);
                        newProjectTask.Code = _projectTaskRepo.GenerateProjectTaskCodeTime(projectTask.TypeProjectItem ?? 1).Trim();
                    }
                    

                    if (projectTask.EmployeeIDRequest > 0)
                    {
                        newProjectTask.IsApproved = 1; // Chờ duyệt,
                    }
                    else
                    {
                        newProjectTask.IsApproved = 0; // chưa duyệt
                    }
                    if (await _projectItemRepo.CreateAsync(newProjectTask) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to create project task."));
                    }

                    var listEmail = new List<String>();

                    if (projectTask.Employee != null && projectTask.Employee.Count > 0)
                    {
                        foreach (var emp in projectTask.Employee)
                        {
                            var projectTaskEmployee = new ProjectTaskEmployee
                            {
                                ProjectTaskID = newProjectTask.ID,
                                Type = 1, // 1: assignee 
                                EmployeeID = emp,
                                IsDeleted = false
                            };
                            var employeeValue = await _employeeRepo.GetByIDAsync(emp);

                            if (emp != currentUser.EmployeeID)
                            {
                                listEmail.Add(employeeValue.EmailCongTy);
                            }
                            if (await _projectTaskEmployeeRepo.CreateAsync(projectTaskEmployee) <= 0)
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, "Failed to assign employee to project task."));
                            }
                        }
                    }
                    List<UserTeam> leaders = new List<UserTeam>();
                    if (projectTask.IsPersonalProject != true)
                    {

                        string usersString = "";

                        if (projectTask.Employee.Count > 0)
                        {
                            usersString = string.Join(",", projectTask.Employee);

                        }

                        var param = new
                        {
                            p_UserID = usersString
                        };
                        leaders = await SqlDapper<UserTeam>.ProcedureToListTAsync("spGetLeaderTeam", param);
                    }



                    if (projectTask.EmployeeRelate != null && projectTask.EmployeeRelate.Count > 0)
                    {
                        if (!projectTask.EmployeeRelate.Contains(currentUser.EmployeeID) && !projectTask.Employee.Contains(currentUser.EmployeeID) && projectTask.EmployeeIDRequest != currentUser.EmployeeID)
                        {
                            projectTask.EmployeeRelate.Add(currentUser.EmployeeID);
                        }



                        if (leaders.Count > 0)
                        {
                            foreach (var item in leaders)
                            {
                                if (item.LeaderID != null && !projectTask.EmployeeRelate.Contains(item.LeaderID ?? -1))
                                {
                                    projectTask.EmployeeRelate.Add(item.LeaderID ?? 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!projectTask.Employee.Contains(currentUser.EmployeeID) && projectTask.EmployeeIDRequest != currentUser.EmployeeID)
                        {
                            projectTask.EmployeeRelate = new List<int>();
                            projectTask.EmployeeRelate.Add(currentUser.EmployeeID);
                        }

                        if (leaders.Count > 0)
                        {
                            foreach (var item in leaders)
                            {
                                if (item.LeaderID != null && !projectTask.EmployeeRelate.Contains(item.LeaderID ?? -1))
                                {
                                    projectTask.EmployeeRelate.Add(item.LeaderID ?? 0);
                                }
                            }
                        }
                    }
                    var listEmployeeRelate = new List<string>();
                    foreach (var emp in projectTask.EmployeeRelate)
                    {
                        var employeeValue = await _employeeRepo.GetByIDAsync(emp);
                        if (employeeValue != null)
                        {
                            listEmployeeRelate.Add(employeeValue.EmailCongTy);
                        }
                        var projectTaskEmployee = new ProjectTaskEmployee
                        {
                            ProjectTaskID = newProjectTask.ID,
                            Type = 2, // 2: related 
                            EmployeeID = emp,
                            IsDeleted = false
                        };
                        if (await _projectTaskEmployeeRepo.CreateAsync(projectTaskEmployee) <= 0)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, "Failed to assign employee relate to project task."));
                        }
                    }


                    string listEmployeeRelateString = string.Join(",", listEmployeeRelate.Distinct());
                    string listToEmailString = string.Join(",", listEmail.Distinct());
                    var employeeAsigner = await _employeeRepo.GetByIDAsync(newProjectTask.EmployeeIDRequest ?? 0);
                    if (listEmployeeRelateString.Count() > 0)
                    {
                        SendEmailReceiveProjectTaskParam sendEmailContent = new SendEmailReceiveProjectTaskParam
                        {
                            emailRecive = listToEmailString,
                            projectTaskName = newProjectTask.Mission,
                            projectTaskCode = newProjectTask.Code,
                            nameAsigner = employeeAsigner.FullName,
                            startDate = newProjectTask.PlanStartDate?.ToString("dd/MM/yyyy") ?? "",
                            endDate = newProjectTask.PlanEndDate?.ToString("dd/MM/yyyy") ?? "",
                            discription = newProjectTask.Description,
                            emailCC = listEmployeeRelateString
                        };
                        _ = Task.Run(() => _sendEmailService.SendEmailReceiveProjectTask(sendEmailContent));
                    }

                    if (projectTask.ProjectTaskChecklists != null && projectTask.ProjectTaskChecklists.Count > 0)
                    {
                        foreach (var checklist in projectTask.ProjectTaskChecklists)
                        {
                            var projectTaskChecklist = await _checklistRepo.GetByIDAsync(checklist);
                            projectTaskChecklist.ProjectTaskID = newProjectTask.ID;
                            if (await _checklistRepo.UpdateAsync(projectTaskChecklist) <= 0)
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, "Failed to update project task checklist."));
                            }
                        }
                    }

                    if (projectTask.Links != null && projectTask.Links.Count > 0)
                    {
                        foreach (var link in projectTask.Links)
                        {
                            var projectTaskAttachment = await _attachmentRepo.GetByIDAsync(link);
                            projectTaskAttachment.ProjectTaskID = newProjectTask.ID;
                            if (await _attachmentRepo.UpdateAsync(projectTaskAttachment) <= 0)
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, "Failed to update project task attachment."));
                            }
                        }
                    }

                    if (projectTask.Files != null && projectTask.Files.Count > 0)
                    {
                        foreach (var file in projectTask.Files)
                        {
                            var projectTaskAttachment = await _attachmentRepo.GetByIDAsync(file);
                            projectTaskAttachment.ProjectTaskID = newProjectTask.ID;
                            if (await _attachmentRepo.UpdateAsync(projectTaskAttachment) <= 0)
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, "Failed to update project task attachment."));
                            }
                        }
                    }

                    var newProjectTaskLog = new ProjectTaskLog
                    {
                        ProjectTaskID = newProjectTask.ID,
                        TypeLog = "Tạo mới công việc", // Duyệt công việc
                        ContentLog = $"- {currentUser.FullName} đã tạo mới công việc ngày {DateTime.Now.ToString("dd/MM/yyyy")}."
                    };

                    await _projectTaskLogRepo.CreateAsync(newProjectTaskLog);

                    return Ok(ApiResponseFactory.Success(newProjectTask));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to create project task."));
            }
        }



        [HttpPost("Approve")]
        public async Task<IActionResult> ApproveTask([FromBody] List<int> projectTaskIDs, bool isApproved, string? review)
        {
            try
            {
                if (projectTaskIDs == null || projectTaskIDs.Count <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Project task approve data is null."));
                }
                List<ProjectTaskApprove> result = new List<ProjectTaskApprove>();
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                foreach (var taskApprove in projectTaskIDs)
                {
                    var exitProjectTask = _projectItemRepo.GetByID(taskApprove);

                    #region GỬI MAIL KHI DUYỆT CÔNG VIỆC




                    var listEmail1 = new List<String>();

                    var listEmployee = _projectTaskEmployeeRepo.GetAll(x => x.ProjectTaskID == taskApprove && x.Type == 1).Select(x => x.EmployeeID).ToList();

                    if (listEmployee != null && listEmployee.Count > 0)
                    {
                        foreach (var emp in listEmployee)
                        {
                            var employeeValue = await _employeeRepo.GetByIDAsync(emp ?? 0);

                            if (emp != currentUser.EmployeeID)
                            {
                                listEmail1.Add(employeeValue.EmailCongTy);
                            }

                        }
                    }
                    List<UserTeam> leaders1 = new List<UserTeam>();



                    string usersString = "";

                    if (listEmployee.Count > 0)
                    {
                        usersString = string.Join(",", listEmployee.Distinct());

                    }

                    var param = new
                    {
                        p_UserID = usersString
                    };
                    leaders1 = await SqlDapper<UserTeam>.ProcedureToListTAsync("spGetLeaderTeam", param);

                    var listEmployeeRelate = _projectTaskEmployeeRepo.GetAll(x => x.ProjectTaskID == taskApprove && x.Type == 2).Select(x => x.EmployeeID).ToList();

                    if (listEmployeeRelate != null && listEmployeeRelate.Count > 0)
                    {
                        if (!listEmployeeRelate.Contains(currentUser.EmployeeID) && !listEmployeeRelate.Contains(currentUser.EmployeeID) && exitProjectTask.EmployeeIDRequest != currentUser.EmployeeID)
                        {
                            listEmployeeRelate.Add(currentUser.EmployeeID);
                        }


                        if (leaders1.Count > 0)
                        {
                            foreach (var item in leaders1)
                            {
                                if (item.LeaderID != null && !listEmployeeRelate.Contains(item.LeaderID ?? -1))
                                {
                                    listEmployeeRelate.Add(item.LeaderID ?? 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!listEmployee.Contains(currentUser.EmployeeID) && exitProjectTask.EmployeeIDRequest != currentUser.EmployeeID)
                        {
                            listEmployeeRelate.Add(currentUser.EmployeeID);
                        }

                        if (leaders1.Count > 0)
                        {
                            foreach (var item in leaders1)
                            {
                                if (item.LeaderID != null && !listEmployeeRelate.Contains(item.LeaderID ?? -1))
                                {
                                    listEmployeeRelate.Add(item.LeaderID ?? 0);
                                }
                            }
                        }
                    }
                    var listEmployeeRelate2 = new List<string>();
                    foreach (var emp in listEmployeeRelate)
                    {
                        var employeeValue = await _employeeRepo.GetByIDAsync(emp ?? 0);
                        if (employeeValue != null)
                        {
                            listEmployeeRelate2.Add(employeeValue.EmailCongTy);
                        }
                    }


                    string listEmployeeRelateString1 = string.Join(",", listEmployeeRelate2.Distinct());
                    string listToEmailString1 = string.Join(",", listEmail1.Distinct());
                    var employeeAsigner1 = await _employeeRepo.GetByIDAsync(exitProjectTask.EmployeeIDRequest ?? 0);


                    if (listEmployeeRelateString1.Count() > 0)
                    {
                        SendEmailReceiveProjectTaskParam sendEmailContent1 = new SendEmailReceiveProjectTaskParam
                        {
                            emailRecive = listToEmailString1,
                            emailCC = listEmployeeRelateString1,
                            projectTaskName = exitProjectTask.Mission,
                            projectTaskCode = exitProjectTask.Code,
                            nameAsigner = employeeAsigner1.FullName,
                            startDate = exitProjectTask.PlanStartDate?.ToString("dd/MM/yyyy") ?? "",
                            endDate = exitProjectTask.PlanEndDate?.ToString("dd/MM/yyyy") ?? "",
                            completedDate = DateTime.Now.ToString("dd/MM/yyyy"),
                            reviewDate = DateTime.Now.ToString("dd/MM/yyyy"),
                            reviewDiscription = review,
                        };
                        if (isApproved)
                        {
                            _ = Task.Run(() => _sendEmailService.SendEmailCompleteReviewProjectTask(sendEmailContent1));

                        }
                        else
                        {
                            _ = Task.Run(() => _sendEmailService.SendEmailRejectReviewProjectTask(sendEmailContent1));

                        }
                    }

                    #endregion


                    if (exitProjectTask == null || exitProjectTask.ID <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Can't fount project task."));
                    }
                    if (exitProjectTask.Status != 3)
                    {
                        continue; // chỉ duyệt những công việc chưa hoàn thành
                    }
                    var newApproveTask = new ProjectTaskApprove
                    {
                        ProjectTaskID = taskApprove,
                        IsApprove = isApproved,
                        Review = review,
                        EmployeeID = currentUser.EmployeeID,
                    };

                    if (await _projectTaskApproveRepo.CreateAsync(newApproveTask) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to approve project task."));
                    }

                    if (isApproved)
                    {
                        exitProjectTask.IsApproved = 2;
                    }
                    else
                    {
                        exitProjectTask.IsApproved = 3;
                    }

                    if (exitProjectTask.Status <= 1)
                    {
                        if (exitProjectTask.ActualEndDate < DateTime.Now)
                        {
                            exitProjectTask.Status = 3;
                        }
                        else
                        {
                            exitProjectTask.Status = 2;
                        }
                    }
                    exitProjectTask.UpdatedDate = DateTime.Now;

                    var newProjectTaskLog = new ProjectTaskLog
                    {
                        ProjectTaskID = taskApprove,
                        TypeLog = "Duyệt công việc", // Duyệt công việc

                    };
                    if (review.IsNullOrEmpty())
                    {
                        newProjectTaskLog.ContentLog = $"- {currentUser.FullName} đã {(isApproved ? "duyệt hoàn thành" : "từ chối duyệt")} công việc.";
                    }
                    else
                    {
                        newProjectTaskLog.ContentLog = $"- {currentUser.FullName} đã {(isApproved ? "duyệt hoàn thành" : "từ chối duyệt")} công việc với nhận xét: {review}.";
                    }


                    await _projectTaskLogRepo.CreateAsync(newProjectTaskLog);

                    if (await _projectItemRepo.UpdateAsync(exitProjectTask) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to approve project task."));
                    }

                    result.Add(newApproveTask);
                }
                return Ok(ApiResponseFactory.Success(result));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to approve project task."));
            }
        }

        [HttpPost("import_excel")]
        public async Task<IActionResult> ImportExcell([FromBody] List<ImportExcellProjectTaskParam> request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                foreach (var item in request)
                {
                    if (item.TT == null || item.TT.Trim().Count() <= 0 ||
                        item.Mission == null || item.Mission.Trim().Count() < 1 ||
                        item.ProjectCode == null || item.ProjectCode.Trim().Count() < 1 ||
                        item.EmployeeCode == null || item.EmployeeCode.Trim().Count() < 1 ||
                        item.AssigneesCodes == null || item.AssigneesCodes.Trim().Count() < 1 ||
                        //item.RelateEmployee == null || item.RelateEmployee.Trim().Count() < 1 ||
                        item.TaskComplexity <= 0
                        )
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Thiếu thông tin bắt buộc"));
                    }

                    if (item.PlanStartDate != null && item.PlanEndDate != null && item.PlanStartDate > item.PlanEndDate)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Ngày bắt đầu dự kiến phải nhỏ hơn ngày kết thúc dự kiến"));
                    }

                    if (item.TaskComplexity == null || item.TaskComplexity < 1 || item.TaskComplexity > 5)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Mức độ phức tạp phải từ 1 đến 5"));
                    }
                }

                //var listProjectTaskParend = request.Where(x => !x.TT.Contains(".")).ToList();

                var codeToID = new Dictionary<string, int>();

                var listProject = _projectRepo.GetAll(x => x.IsDeleted != true).ToList();
                var listEmployee = _employeeRepo.GetAll().ToList();

                foreach (var item in request)
                {
                    // lấy ID dự án theo mã dự án
                    var projectID = listProject.FirstOrDefault(x => x.ProjectCode.Trim() == item.ProjectCode.Trim())?.ID;
                    if (projectID == null || projectID <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy dự án với mã dự án {item.ProjectCode}"));
                    }
                    // Lấy ID Người giao việc theo mã nhân viên
                    var employeeAsignerID = listEmployee.FirstOrDefault(x => x.Code.Trim() == item.EmployeeCode.Trim())?.ID;
                    if (employeeAsignerID == null || employeeAsignerID <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy người giao việc với mã nhân viên {item.EmployeeCode}"));
                    }
                    // Lấy List ID Người thực hiện theo list mã nhân viên
                    var listEmployeeAssigneeIDString = item.AssigneesCodes.Replace(" ", "");
                    var listEmployeeAssigneeID = listEmployeeAssigneeIDString.Split(',').ToList();
                    var employeeAssigneeIDs = listEmployee.Where(x => listEmployeeAssigneeID.Contains(x.Code.Trim())).Select(x => x.ID).ToList();
                    if (employeeAssigneeIDs == null || employeeAssigneeIDs.Count <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy người thực hiện với mã nhân viên {item.AssigneesCodes}"));
                    }
                    // Lấy List ID Người liên quan theo list mã nhân viên
                    var listEmployeeRelateIDString = item.RelatedCodes.Replace(" ", "");
                    var listEmployeeRelateID = listEmployeeRelateIDString.Split(',').ToList();
                    var employeeRelateIDs = listEmployee.Where(x => listEmployeeRelateID.Contains(x.Code.Trim())).Select(x => x.ID).ToList();

                    // Lấy ID leader trực tiếp theo người thực hiện

                    List<UserTeam> leaders = new List<UserTeam>();
                    string usersString = "";
                    usersString = string.Join(",", employeeAssigneeIDs);
                    //leaders = SQLHelper<UserTeam>.ProcedureToListModel("spGetLeaderTeam", new string[] { "@p_UserID" }, new object[] { usersString });
                    var param = new
                    {
                        p_UserID = usersString
                    };
                    leaders = await SqlDapper<UserTeam>.ProcedureToListTAsync("spGetLeaderTeam", param);

                    // Nếu người liên quan không có người thực hiện và người giao việc thì thêm người đang đăng nhập vào người liên quan

                    if (employeeRelateIDs != null && employeeRelateIDs.Count > 0)
                    {
                        if (!employeeAssigneeIDs.Contains(currentUser.EmployeeID) && !employeeAssigneeIDs.Contains(currentUser.EmployeeID) && employeeAsignerID != currentUser.EmployeeID)
                        {
                            employeeRelateIDs.Add(currentUser.EmployeeID);
                        }
                    }
                    else
                    {
                        if (!employeeAssigneeIDs.Contains(currentUser.EmployeeID) && employeeAsignerID != currentUser.EmployeeID)
                        {
                            employeeRelateIDs.Add(currentUser.EmployeeID);
                        }
                    }

                    // Nếu người liên quan không có leader trực tiếp của người thực hiện thì thêm leader trực tiếp vào người liên quan

                    if (leaders.Count > 0)
                    {
                        foreach (var leader in leaders)
                        {
                            if (leader.LeaderID != null && !employeeRelateIDs.Contains(leader.LeaderID ?? -1))
                            {
                                employeeRelateIDs.Add(leader.LeaderID ?? 0);
                            }
                        }
                    }

                    var trimedTT = item.TT.Trim('.');
                    int lastDot = trimedTT.LastIndexOf('.');
                    string parentTT = lastDot > 0 ? trimedTT.Substring(0, lastDot) : null;
                    int? parentID = null;
                    if (parentTT != null && codeToID.ContainsKey(parentTT))
                    {
                        parentID = codeToID[parentTT];
                    }


                    var createProjectTask = new ProjectItem
                    {
                        Mission = item.Mission,
                        Code = _projectItemRepo.GenerateProjectItemCode(projectID ?? 0),
                        ProjectID = projectID,
                        EmployeeIDRequest = employeeAsignerID,
                        TypeProjectItem = 1,
                        ParentID = parentID,
                        TaskComplexity = item.TaskComplexity,
                        Status = 1,
                        IsApproved = 1,
                        PlanStartDate = item.PlanStartDate,
                        PlanEndDate = item.PlanEndDate,
                        EmployeeCreateID = currentUser.EmployeeID
                    };
                    if (await _projectItemRepo.CreateAsync(createProjectTask) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, $"Failed to create project task with TT {item.TT}"));
                    }

                    var newProjectTaskLog = new ProjectTaskLog
                    {
                        ProjectTaskID = createProjectTask.ID,
                        TypeLog = "Tạo mới công việc", // Duyệt công việc
                        ContentLog = $"- {currentUser.FullName} đã tạo mới công việc bằng phương pháp nhập Excell ngày {DateTime.Now.ToString("dd/MM/yyyy")}."
                    };

                    await _projectTaskLogRepo.CreateAsync(newProjectTaskLog);

                    foreach (var item1 in employeeAssigneeIDs)
                    {
                        var newEmployeeAssignee = new ProjectTaskEmployee()
                        {
                            EmployeeID = item1,
                            ProjectTaskID = createProjectTask.ID,
                            Type = 1,
                        };
                        if (await _projectTaskEmployeeRepo.CreateAsync(newEmployeeAssignee) <= 0)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, $"Failed to assign employee to project task with TT {item.TT}"));
                        }
                    }

                    if (employeeRelateIDs != null && employeeRelateIDs.Count > 0)
                    {
                        foreach (var item2 in employeeRelateIDs)
                        {
                            var newEmployeeRelate = new ProjectTaskEmployee()
                            {
                                EmployeeID = item2,
                                ProjectTaskID = createProjectTask.ID,
                                Type = 2,
                            };
                            if (await _projectTaskEmployeeRepo.CreateAsync(newEmployeeRelate) <= 0)
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, $"Failed to assign relate employee to project task with TT {item.TT}"));
                            }

                        }
                    }
                    codeToID.Add(item.TT.Trim(), createProjectTask.ID);
                }
                return Ok(ApiResponseFactory.Success(null, "Import excel project task successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
