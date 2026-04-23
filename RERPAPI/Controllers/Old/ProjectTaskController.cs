using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Spreadsheet;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        ProjectTaskEmailBandRepo _projectTaskEmailBandRepo;
        ProjectTaskAttendanceRepo _projectTaskAttendanceRepo;
        ProjectTaskSettingRepo _projectTaskSettingRepo;
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
            DepartmentRepo departmentRepo,
            ProjectTaskEmailBandRepo projectTaskEmailBandRepo,
            ProjectItemRepo projectItemRepo,
            ProjectTaskAttendanceRepo projectTaskAttendanceRepo,
            ProjectTaskSettingRepo projectTaskSettingRepo
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
            _projectTaskEmailBandRepo = projectTaskEmailBandRepo;
            _projectTaskAttendanceRepo = projectTaskAttendanceRepo;
            _projectTaskSettingRepo = projectTaskSettingRepo;
        }


        public class MoveRequest
        {
            public int ProjectTaskGroupID { get; set; }
            public int OrderIndex { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectTask(DateTime dateStart, DateTime dateEnd, int status, int viewNumber = 1)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                dateStart = dateStart.Date;
                dateEnd = dateEnd.Date.AddDays(1).AddSeconds(-1);

                if (currentUser.EmployeeID == 0)
                {
                    var param1 = new
                    {
                        EmployeeID = -1,
                        //EmployeeID = 610,
                        DateStart = dateStart,
                        DateEnd = dateEnd,
                        Status = status,
                        ViewNumber = viewNumber
                    };
                    var projectTasksnew = await SqlDapper<spGetProjectTaskByEmployeeID>.ProcedureToListTAsync("spGetProjectTaskByEmployeeID", param1);
                    return Ok(ApiResponseFactory.Success(new
                    {
                        ProjectTask = projectTasksnew.OrderByDescending(x => x.UpdatedDate),
                        UserID = currentUser.EmployeeID
                    }));
                }
                var param = new
                {
                    EmployeeID = currentUser.EmployeeID,
                    //EmployeeID = 610,
                    DateStart = dateStart,
                    DateEnd = dateEnd,
                    Status = status,
                    ViewNumber = viewNumber
                };
                var projectTasks = await SqlDapper<spGetProjectTaskByEmployeeID>.ProcedureToListTAsync("spGetProjectTaskByEmployeeID", param);
                return Ok(ApiResponseFactory.Success(new
                {
                    ProjectTask = projectTasks.OrderByDescending(x => x.UpdatedDate),
                    UserID = currentUser.EmployeeID
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to retrieve project tasks."));
            }
        }

        [HttpGet("number-overdue")]
        public async Task<IActionResult> GetCountOverdueProjectTasks()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                var param = new
                {
                    EmployeeID = currentUser.EmployeeID
                };
                var result = await SqlDapper<object>.ProcedureToListTAsync("spGetCountOverdueProjectTasks", param);
                return Ok(ApiResponseFactory.Success(new
                {
                    result
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
                    p_EmployeeID = currentUser.EmployeeID
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

        // -- Get project task time line by Team (dùng cho lấy dữ liệu hiển thị ở timeline) ---
        [HttpGet("project-task-timeline-by-team")]
        public async Task<IActionResult> GetProjectTaskTimeLineByTeam(
            [FromQuery] DateTime dateStart,
            [FromQuery] DateTime dateEnd,
            [FromQuery] int departmentID = 0,
            [FromQuery] int teamID = 0,
            [FromQuery] int userID = 0,
            [FromQuery] int projectID = 0
            )
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                dateStart = dateStart.Date;
                dateEnd = dateEnd.Date.AddDays(1).AddSeconds(-1);
                var param = new
                {
                    DateStart = dateStart,
                    DateEnd = dateEnd,
                    DepartmentID = departmentID,
                    TeamID = teamID,
                    UserID = userID,
                    ProjectID = projectID
                };
                var projectTasks = await SqlDapper<object>.ProcedureToListAsync("spGetProjectTaskTimeLineByTeam", param);
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

        // -- Get project task view status dùng cho chart ---
        [HttpGet("project-task-view-status-chart")]
        public async Task<IActionResult> GetProjectTaskViewStatusForChart(
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
                var projectTasks = await SqlDapper<object>.ProcedureToListAsync("spGetSummarizeProjectTaskForChart", param);
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
        public async Task<IActionResult> getAllProject()
        {
            try
            {
                DateTime minDate = new DateTime(DateTime.Now.Year - 1, 1, 1);

                var param = new
                {
                    DateStart = minDate
                };
                var result = await SqlDapper<object>.ProcedureToListAsync("spGetProjectForProjectTask", param);

                //var result = _projectRepo.GetAll(x => x.CreatedDate >= minDate && x.IsDeleted == false).OrderByDescending(x => x.ID);
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

        [HttpGet("project-task-child")]
        public async Task<IActionResult> GetProjectTaskChild(int projectTaskID)
        {
            try
            {
                var param = new
                {
                    ID = projectTaskID
                };
                var list = await SqlDapper<object>.ProcedureToListTAsync("spGetProjectTaskChild", param);
                return Ok(ApiResponseFactory.Success(list));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to retrieve project task child."));
            }
        }

        [HttpPost("project-task-child")]
        public async Task<IActionResult> AddProjectTaskChild([FromBody] ProjectTaskChildParam item)
        {
            try
            {
                if (item.ID > 0 && item.IsDeletedFromParent == true)
                {
                    var existingTask = await _projectItemRepo.GetByIDAsync(item.ID);
                    if (existingTask == null || existingTask.ID <= 0)
                    {
                        return NotFound(ApiResponseFactory.Fail(null, "Project task not found."));
                    }
                    var param = new
                    {
                        Id = existingTask.ID,
                        Col = "ParentID"
                    };
                    var result = await SqlDapper<UserTeam>.ExecuteStoredProcedure("spUpdateDateToNull", param);

                    return Ok(ApiResponseFactory.Success());
                }
                else
                {
                    var newProjectTask = new ProjectItem
                    {
                        ProjectID = item.ProjectID,
                        Mission = item.Mission,
                        PlanStartDate = item.PlanStartDate,
                        PlanEndDate = item.PlanEndDate.HasValue ? item.PlanEndDate.Value.Date.AddDays(1).AddSeconds(-1) : (DateTime?)null,
                        ParentID = item.ParentID,
                        TypeProjectItem = item.TypeProjectItem,
                        EmployeeIDRequest = item.EmployeeIDRequest,
                        TaskComplexity = item.TaskComplexity,
                        ProjectTaskTypeID = item.ProjectTaskTypeID,
                        Status = 0
                    };

                    // Tạo mã code cho công việc
                    if (item.ProjectID != null && item.ProjectID > 0)
                    {
                        newProjectTask.Code = _projectItemRepo.GenerateProjectItemCode(item.ProjectID ?? 0).Trim();

                    }
                    else
                    {
                        //var employeeById = _employeeRepo.GetByID(currentUser.EmployeeID);
                        newProjectTask.Code = _projectTaskRepo.GenerateProjectTaskCodeTime(item.ProjectTaskTypeID ?? 1).Trim();
                    }

                    if (item.EmployeeAssigneeID != null && item.EmployeeAssigneeID > 0)
                    {
                        Employee userAssignee = await _employeeRepo.GetByIDAsync(item.EmployeeAssigneeID ?? 0);
                        newProjectTask.UserID = userAssignee.UserID;

                        if (newProjectTask.UserID == null || newProjectTask.UserID < 0)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, "User Is Null, please chose user!"));
                        }
                    }



                    if (await _projectItemRepo.CreateAsync(newProjectTask) > 0)
                    {
                        var newEmployee = new ProjectTaskEmployee
                        {
                            ProjectTaskID = newProjectTask.ID,
                            EmployeeID = item.EmployeeAssigneeID ??0,
                            Type = 1
                        };


                        await _projectTaskEmployeeRepo.CreateAsync(newEmployee);

                        string usersString = string.Join(",", item.EmployeeAssigneeID);

                        var param = new
                        {
                            p_UserID = usersString
                        };
                        var leaders = await SqlDapper<UserTeam>.ProcedureToListTAsync("spGetLeaderTeam", param);

                        if (leaders.Count > 0)
                        {
                            var newEmployeeRelate = new ProjectTaskEmployee
                            {
                                ProjectTaskID = newProjectTask.ID,
                                EmployeeID = (int)leaders[0].LeaderID,
                                Type = 2
                            };
                            await _projectTaskEmployeeRepo.CreateAsync(newEmployeeRelate);
                        }
                        return Ok(ApiResponseFactory.Success(newProjectTask));

                    }
                    return BadRequest(ApiResponseFactory.Fail(null, "Failed to create child project task."));

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to create child project task."));
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
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var existing = await _checklistRepo.GetByIDAsync(id);
                if (existing == null) return NotFound(ApiResponseFactory.Fail(null, "Checklist item not found."));

                existing.ChecklistTitle = item.ChecklistTitle ?? existing.ChecklistTitle;
                existing.IsDone = item.IsDone ?? existing.IsDone;
                existing.OrderIndex = item.OrderIndex;
                var newProjectTaskChangeCheckListLog = new ProjectTaskLog
                {
                    ProjectTaskID = existing.ProjectTaskID,
                    TypeLog = "Thay đổi checklist",
                    ContentLog = ""
                };
                if (existing.ChecklistTitle != item.ChecklistTitle)
                {
                    newProjectTaskChangeCheckListLog.ContentLog = $"- {currentUser.FullName} đã thay đổi nội dung checklist từ: {existing.ChecklistTitle} thành: {item.ChecklistTitle}. \\n";
                }

                if (item.IsDeleted ?? false)
                {
                    newProjectTaskChangeCheckListLog.ContentLog += $"- {currentUser.FullName} đã xóa check list {existing.ChecklistTitle}";
                }
                await _projectTaskLogRepo.CreateAsync(newProjectTaskChangeCheckListLog);


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
                    //if (employeeType == 1)
                    //{
                    //    var projectTask = await _projectTaskRepo.GetByIDAsync(projectTaskID);
                    //    var employeeValue = await _employeeRepo.GetByIDAsync(projectTask.EmployeeID ?? 0);
                    //    SendEmailReceiveProjectTaskParam sendEmailContent = new SendEmailReceiveProjectTaskParam
                    //    {
                    //        projectTaskName = projectTask.Title,
                    //        projectTaskCode = projectTask.Code,
                    //        nameAsigner = employeeValue.FullName,
                    //        startDate = projectTask.PlanStartDate?.ToString("dd/MM/yyyy") ?? "",
                    //        endDate = projectTask.PlanEndDate?.ToString("dd/MM/yyyy") ?? "",
                    //        discription = projectTask.Description,
                    //    };
                    //    await _sendEmailService.SendEmailReceiveProjectTask(sendEmailContent);
                    //}
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
                var lstEmployeeTask = _projectTaskEmployeeRepo.GetAll(x => x.ProjectTaskID == taskId && (!x.IsDeleted ?? true) && (x.Type == typeEmployee)).OrderBy(x => x.ID);

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

        #region Email band (Dùng để lưu cấu hình bật tắt nhận email thông báo về công việc của từng người)

        [HttpGet("email-band")]
        public async Task<IActionResult> GetDataEmailBand()
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var data = _projectTaskSettingRepo.GetAll(x => x.EmployeeID == currentUser.EmployeeID).FirstOrDefault();
                return Ok(ApiResponseFactory.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to get data email band."));
            }
        }

        [HttpPost("email-band")]
        public async Task<IActionResult> SaveDataEmailBand(bool SendMailCreateProjectTask = true, bool SendFinishProjectTask = true, bool SendApproveProjectTask = true)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var projectTaskSetting = _projectTaskSettingRepo.GetAll(x => x.EmployeeID == currentUser.EmployeeID).FirstOrDefault();


                if (projectTaskSetting != null && projectTaskSetting.ID > 0)
                {

                    projectTaskSetting.SendMailCreateProjectTask = SendMailCreateProjectTask;
                    projectTaskSetting.SendFinishProjectTask = SendFinishProjectTask;
                    projectTaskSetting.SendApproveProjectTask = SendApproveProjectTask;
                    projectTaskSetting.UpdatedDate = DateTime.Now;
                    if (await _projectTaskSettingRepo.UpdateAsync(projectTaskSetting) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to update email data."));
                    }
                    return Ok(ApiResponseFactory.Success(projectTaskSetting));
                }
                else
                {
                    var existingEmail = _employeeRepo.GetByID(currentUser.EmployeeID)?.EmailCongTy;
                    var newProjectTaskSetting = new ProjectTaskSetting
                    {
                        EmployeeID = currentUser.EmployeeID,
                        EmployeeEmail = existingEmail,
                        SendMailCreateProjectTask = SendMailCreateProjectTask,
                        SendFinishProjectTask = SendFinishProjectTask,
                        SendApproveProjectTask = SendApproveProjectTask,
                        IsDeleted = false,
                    };
                    if (await _projectTaskSettingRepo.CreateAsync(newProjectTaskSetting) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to create email data."));
                    }
                    return Ok(ApiResponseFactory.Success(newProjectTaskSetting));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to update email data."));
            }
        }

        #endregion

        #region Attendance (Dùng để lưu lại thông tin chấm công liên quan đến công việc, phục vụ cho việc tính toán hiệu suất làm việc của nhân viên)
        //[HttpGet("attendance")]
        //public async Task<IActionResult> GetDataAttendance(int projectTaskID)
        //{
        //    try
        //    {
        //        var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //        var currentUser = ObjectMapper.GetCurrentUser(claims);

        //        var data = _projectTaskEmailBandRepo.GetAll(x => x.EmployeeID == currentUser.EmployeeID).FirstOrDefault();
        //        return Ok(ApiResponseFactory.Success(data));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, "Failed to get data email band."));
        //    }
        //}

        [HttpPost("attendance")]
        public async Task<IActionResult> SaveDataAttendance(int projectTaskID, bool isCheck)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                var projectTaskAttendance = _projectTaskAttendanceRepo.GetAll(x => x.ProjectTaskID == projectTaskID && x.EmployeeID == currentUser.EmployeeID && (x.Date.HasValue && x.Date.Value.Date == DateTime.Now.Date)).FirstOrDefault();

                if (projectTaskAttendance != null && projectTaskAttendance.ID > 0)
                {

                    projectTaskAttendance.IsCheck = isCheck;
                    projectTaskAttendance.UpdatedDate = DateTime.Now;
                    if (await _projectTaskAttendanceRepo.UpdateAsync(projectTaskAttendance) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to update data."));
                    }
                    return Ok(ApiResponseFactory.Success(projectTaskAttendance));
                }
                else
                {
                    var newProjectTaskAttendance = new ProjectTaskAttendance
                    {
                        ProjectTaskID = projectTaskID,
                        EmployeeID = currentUser.EmployeeID,
                        IsCheck = isCheck,
                        IsDeleted = false,
                        Date = DateTime.Now.Date
                    };
                    if (await _projectTaskAttendanceRepo.CreateAsync(newProjectTaskAttendance) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to create data."));
                    }
                    return Ok(ApiResponseFactory.Success(newProjectTaskAttendance));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to update data."));
            }
        }
        #endregion

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

                    // tính % thời gian vượt so với kế hoạch nếu có sự thay đổi về ngày bắt đầu hoặc ngày kết thúc thực tế và trạng thái công việc là đã hoàn thành.
                    if (projectTask.Status == 2 && projectTask.ActualEndDate.HasValue
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
                    // log thay đổi thời gian
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
                        existingTask.PlanEndDate = projectTask.PlanEndDate.HasValue ? projectTask.PlanEndDate.Value.Date.AddDays(1).AddSeconds(-1) : (DateTime?)null;

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

                    var oldDate = existingTask.ActualEndDate?.Date;
                    var newDate = projectTask.ActualEndDate?.Date;

                    if (oldDate != newDate)
                    {
                        if (newDate != null && oldDate == null)
                        {
                            newProjectTaskLog.ContentLog +=
                                $"- {currentUser.FullName} đã thêm ngày kết thúc thực tế {newDate:dd/MM/yyyy}. \n";
                            projectTask.Status = 2;
                        }
                        else if (newDate != null && oldDate != null)
                        {
                            newProjectTaskLog.ContentLog +=
                                $"- {currentUser.FullName} đã thay đổi ngày kết thúc thực tế từ {oldDate:dd/MM/yyyy} thành {newDate:dd/MM/yyyy}. \n";
                            projectTask.Status = 2;
                        }
                        else if (newDate == null && oldDate != null)
                        {
                            newProjectTaskLog.ContentLog +=
                                $"- {currentUser.FullName} đã xóa ngày kết thúc thực tế. \n";
                            projectTask.Status = 2;
                        }

                        changeTime++;
                        existingTask.ActualEndDate = newDate;
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

                    var listEmailBand = _projectTaskSettingRepo.GetAll(x => x.SendFinishProjectTask == false).Select(x => x.EmployeeEmail);

                    var listemailRecive = listEmployeeRelate.Except(listEmailBand);
                    var listemailCC = listEmail1.Except(listEmailBand);

                    string listEmployeeRelateString1 = string.Join(",", listemailRecive.Distinct());
                    string listToEmailString1 = string.Join(",", listemailCC.Distinct());
                    var employeeAsigner1 = await _employeeRepo.GetByIDAsync(existingTask.EmployeeIDRequest ?? 0);


                    if (projectTask.Status == 2 && existingTask.Status != projectTask.Status && listToEmailString1.Count() > 0)
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
                    if (existingTask.ProjectID != projectTask.ProjectID)
                    {

                        var newProjectTaskChangeProjectLog = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi dự án",
                            ContentLog = ""
                        };

                        var oldProject = await _projectRepo.GetByIDAsync(existingTask.ProjectID ?? 0);
                        var newProject = await _projectRepo.GetByIDAsync(projectTask.ProjectID ?? 0);


                        // Log change project
                        if (projectTask.ProjectID != null || projectTask.ProjectID > 0)
                        {

                            if (existingTask.ProjectID != null && existingTask.ProjectID > 0)
                            {
                                newProjectTaskChangeProjectLog.ContentLog = $"- {currentUser.FullName} đã thay đổi dự án từ {oldProject.ProjectName} thành {newProject.ProjectName}. \\n";
                                existingTask.Code = _projectItemRepo.GenerateProjectItemCode(projectTask.ProjectID ?? 0).Trim();
                            }
                            else
                            {
                                newProjectTaskChangeProjectLog.ContentLog = $"- {currentUser.FullName} đã thêm vào dự án {newProject.ProjectName}. \\n";
                                existingTask.Code = _projectItemRepo.GenerateProjectItemCode(projectTask.ProjectID ?? 0).Trim();
                            }
                        }
                        else
                        {
                            newProjectTaskChangeProjectLog.ContentLog = $"- {currentUser.FullName} đã xóa dự án {oldProject.ProjectName}. \\n";
                            existingTask.Code = _projectTaskRepo.GenerateProjectTaskCodeTime(projectTask.ProjectTaskTypeID ?? 1).Trim();

                            var param = new
                            {
                                Id = existingTask.ID,
                                Col = "ProjectID"
                            };
                            var result = await SqlDapper<UserTeam>.ExecuteStoredProcedure("spUpdateDateToNull", param);

                        }
                        await _projectTaskLogRepo.CreateAsync(newProjectTaskChangeProjectLog);
                        existingTask.ProjectID = projectTask.ProjectID;

                    }


                    // Log change mission
                    if (existingTask.Mission != projectTask.Mission)
                    {
                        var newProjectTaskChangeMissionLog = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi tên công việc",
                            ContentLog = $"- {currentUser.FullName} đã thay đổi nội dung công việc từ {existingTask.Mission} thành {projectTask.Mission}. \\n"
                        };
                        await _projectTaskLogRepo.CreateAsync(newProjectTaskChangeMissionLog);
                        existingTask.Mission = projectTask.Mission;
                    }


                    if (existingTask.Status != projectTask.Status)
                    {
                        var statusText = new Dictionary<int, string>
                            {
                                { 0, "Chưa làm" },
                                { 1, "Đang làm" },
                                { 2, "Hoàn thành" },
                                { 3, "Pending" }
                            };

                        var oldStatus = statusText.ContainsKey(existingTask.Status ?? 0)
                            ? statusText[existingTask.Status ?? 0]
                            : "Không xác định";

                        var newStatus = statusText.ContainsKey(projectTask.Status ?? 0)
                            ? statusText[projectTask.Status ?? 0]
                            : "Không xác định";

                        var newProjectTaskChangeStatusLog = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi trạng thái",
                            ContentLog = $"- {currentUser.FullName} đã thay đổi trạng thái công việc từ {oldStatus} thành {newStatus}. \n"
                        };

                        await _projectTaskLogRepo.CreateAsync(newProjectTaskChangeStatusLog);

                        existingTask.Status = projectTask.Status;
                    }


                    // Log change Asigner
                    if (existingTask.EmployeeIDRequest != projectTask.EmployeeIDRequest)
                    {

                        var oldAsgner = await _employeeRepo.GetByIDAsync(existingTask.EmployeeIDRequest ?? 0);
                        var newAsgner = await _employeeRepo.GetByIDAsync(projectTask.EmployeeIDRequest ?? 0);
                        var newProjectTaskChangeAsignerLog = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi người giao việc",
                            ContentLog = $"- {currentUser.FullName} đã thay đổi người giao việc từ {oldAsgner.FullName} thành {newAsgner.FullName}. \\n"
                        };
                        await _projectTaskLogRepo.CreateAsync(newProjectTaskChangeAsignerLog);
                        existingTask.EmployeeIDRequest = projectTask.EmployeeIDRequest;
                    }

                    // log change personal project
                    if (existingTask.IsPersonalProject != projectTask.IsPersonalProject)
                    {
                        var oldPersonalProject = (existingTask.IsPersonalProject ?? false) ? "loại công việc dự án" : "loại công việc cá nhân";
                        var newPersonalProject = (projectTask.IsPersonalProject ?? false) ? "loại công việc dự án" : "loại công việc cá nhân";
                        var newProjectTaskChangeStatusPersonalProjectLog = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi loại dự án",
                            ContentLog = $"- {currentUser.FullName} đã thay đổi loại dự án từ {oldPersonalProject} thành {newPersonalProject}. \\n"
                        };
                        existingTask.IsPersonalProject = projectTask.IsPersonalProject;

                    }

                    // log change complexity of task
                    if (existingTask.TaskComplexity != projectTask.TaskComplexity)
                    {
                        var newProjectTaskChangeStatusComplexityLog = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi độ phức tạp công việc",
                            ContentLog = $"- {currentUser.FullName} đã thay đổi độ phức tạp công việc từ {existingTask.TaskComplexity} thành {projectTask.TaskComplexity}. \\n"
                        };
                        await _projectTaskLogRepo.CreateAsync(newProjectTaskChangeStatusComplexityLog);
                        existingTask.TaskComplexity = projectTask.TaskComplexity;

                    }

                    // log change parent task
                    if (existingTask.ParentID != projectTask.ParentID)
                    {
                        var oldParentTask = await _projectItemRepo.GetByIDAsync(existingTask.ParentID ?? 0);
                        var newParentTask = await _projectItemRepo.GetByIDAsync(projectTask.ParentID ?? 0);
                        var newProjectTaskChangeParentLog = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi công việc cha"
                        };
                        if (projectTask.ParentID != null && projectTask.ParentID >= 0)
                        {
                            newProjectTaskChangeParentLog.ContentLog = $"- {currentUser.FullName} đã thay đổi công việc cha từ {(oldParentTask != null ? oldParentTask.Mission : "không có")} thành {(newParentTask != null ? newParentTask.Mission : "không có")}. \\n";
                        }
                        else
                        {
                            newProjectTaskChangeParentLog.ContentLog = $"- {currentUser.FullName} đã xóa công việc cha từ {(oldParentTask != null ? oldParentTask.Mission : "không có")}. \\n";
                            var param = new
                            {
                                Id = existingTask.ID,
                                Col = "ParentID"
                            };
                            var result = await SqlDapper<UserTeam>.ExecuteStoredProcedure("spUpdateDateToNull", param);
                        }

                        await _projectTaskLogRepo.CreateAsync(newProjectTaskChangeParentLog);
                        existingTask.ParentID = projectTask.ParentID;
                    }

                    // log change type project item
                    if (existingTask.TypeProjectItem != projectTask.TypeProjectItem)
                    {
                        var oldTypeProjectItem = _projectTaskTypeRepo.GetByID(existingTask.TypeProjectItem ?? 1);
                        var newTypeProjectItem = _projectTaskTypeRepo.GetByID(projectTask.TypeProjectItem ?? 1);

                        var newProjectTaskChangeTypeProjectItemLog = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi loại hạng mục công việc",
                            ContentLog = $"- {currentUser.FullName} đã thay đổi loại hạng mục công việc từ {oldTypeProjectItem.TypeName} thành {newTypeProjectItem.TypeName}. \\n"
                        };

                        if(projectTask.TypeProjectItem == null || projectTask.TypeProjectItem <= 0)
                        {
                            newProjectTaskChangeTypeProjectItemLog.ContentLog = $"- {currentUser.FullName} đã xóa loại hạng mục công việc từ {oldTypeProjectItem.TypeName}. \\n";
                            var param = new
                            {
                                Id = existingTask.ID,
                                Col = "TypeProjectItem"
                            };
                            var result = await SqlDapper<UserTeam>.ExecuteStoredProcedure("spUpdateDateToNull", param);
                        }
                        await _projectTaskLogRepo.CreateAsync(newProjectTaskChangeTypeProjectItemLog);
                        existingTask.TypeProjectItem = projectTask.TypeProjectItem;
                    }

                    // log change project task type
                    if (existingTask.ProjectTaskTypeID != projectTask.ProjectTaskTypeID)
                    {
                        var oldProjectTaskTypeID = _projectTaskTypeRepo.GetByID(existingTask.ProjectTaskTypeID ?? 1);
                        var newProjectTaskTypeID = _projectTaskTypeRepo.GetByID(projectTask.ProjectTaskTypeID ?? 1);

                        var newProjectTaskChangeProjectTaskTypeIDLog = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi loại công việc",
                            ContentLog = $"- {currentUser.FullName} đã thay đổi loại công việc từ {oldProjectTaskTypeID.TypeName} thành {newProjectTaskTypeID.TypeName}. \\n"
                        };
                        await _projectTaskLogRepo.CreateAsync(newProjectTaskChangeProjectTaskTypeIDLog);
                        existingTask.ProjectTaskTypeID = projectTask.ProjectTaskTypeID;
                    }

                    // log change deadline
                    if (existingTask.Deadline != projectTask.Deadline)
                    {
                        var newProjectTaskChangeDeadlineLog = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi deadline",
                            ContentLog = $"- {currentUser.FullName} đã thay đổi deadline từ {existingTask.Deadline?.ToString("dd/MM/yyyy")} thành {projectTask.Deadline?.ToString("dd/MM/yyyy")}. \\n"
                        };
                        await _projectTaskLogRepo.CreateAsync(newProjectTaskChangeDeadlineLog);
                        existingTask.Deadline = projectTask.Deadline;
                    }

                    // log change Priority
                    if (existingTask.Priority != projectTask.Priority)
                    {
                        var newProjectTaskChangePriority = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi độ ưu tiên",
                            ContentLog = $"- {currentUser.FullName} đã thay đổi độ ưu tiên từ {existingTask.Priority ?? 1} thành {projectTask.Priority ?? 1}. \\n"
                        };
                        await _projectTaskLogRepo.CreateAsync(newProjectTaskChangePriority);
                        existingTask.Priority = projectTask.Priority;
                    }

                    // log change EstimatedTime
                    if (existingTask.EstimatedTime != projectTask.EstimatedTime)
                    {
                        var newProjectTaskChangeEstimatedTime = new ProjectTaskLog
                        {
                            ProjectTaskID = existingTask.ID,
                            TypeLog = "Thay đổi thời gian dự kiến",
                            ContentLog = $"- {currentUser.FullName} đã thay đổi độ ưu tiên từ {existingTask.EstimatedTime} thành {projectTask.EstimatedTime}. \\n"
                        };
                        await _projectTaskLogRepo.CreateAsync(newProjectTaskChangeEstimatedTime);
                        existingTask.EstimatedTime = projectTask.EstimatedTime;
                    }

                    existingTask.IsAdditional = projectTask.IsAdditional;
                    existingTask.Description = projectTask.Description;
                    existingTask.DescriptionSolution = projectTask.DescriptionSolution;
                    existingTask.ProjectTaskResult = projectTask.ProjectTaskResult;
                    existingTask.ProjectTaskTypeID = projectTask.ProjectTaskTypeID;
                    existingTask.NeedApprove = projectTask.NeedApprove;
                    if (projectTask.Employee != null && projectTask.Employee.Count > 0)
                    {
                        Employee userAssignee = await _employeeRepo.GetByIDAsync(projectTask.Employee[0]);
                        if (userAssignee.ID >= 0 && existingTask.UserID != userAssignee.UserID)
                        {
                            existingTask.UserID = userAssignee.UserID;
                        }
                    }

                    if (existingTask.IsApproved == null || existingTask.IsApproved <= 1)
                    {

                        existingTask.IsApproved = 1; // Chờ duyệt,

                    }
                    if (existingTask.UserID == null || existingTask.UserID < 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "User Is Null, please chose user!"));
                    }
                    existingTask.UpdatedDate = DateTime.Now;
                    if (await _projectItemRepo.UpdateAsync(existingTask) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to update project task."));
                    }

                    // Xóa ngày bắt đầu thực tế
                    if (projectTask.ActualStartDate == null && existingTask.ActualStartDate?.Date != projectTask.ActualStartDate?.Date)
                    {
                        var param = new
                        {
                            Id = existingTask.ID,
                            Col = "ActualStartDate"
                        };
                        var result = await SqlDapper<UserTeam>.ExecuteStoredProcedure("spUpdateDateToNull", param);
                    }

                    // Xóa ngày kết thúc thực tế
                    if (projectTask.ActualEndDate == null && existingTask.ActualEndDate?.Date != projectTask.ActualEndDate?.Date)
                    {
                        var param = new
                        {
                            Id = existingTask.ID,
                            Col = "ActualEndDate"
                        };
                        var result = await SqlDapper<UserTeam>.ExecuteStoredProcedure("spUpdateDateToNull", param);
                    }

                    // Xóa số giờ dự kiến
                    if ((projectTask.EstimatedTime == null || projectTask.EstimatedTime == 0) && existingTask.EstimatedTime != projectTask.EstimatedTime)
                    {
                        var param = new
                        {
                            Id = existingTask.ID,
                            Col = "EstimatedTime"
                        };
                        var result = await SqlDapper<UserTeam>.ExecuteStoredProcedure("spUpdateDateToNull", param);
                    }

                    if (!(projectTask.NeedApprove ?? true) && projectTask.Status == 2)
                    {
                        var newProjectTaskApprove = new ProjectTaskApprove
                        {
                            ProjectTaskID = existingTask.ID,
                            IsApprove = true,
                            EmployeeID = currentUser.EmployeeID,
                            //Review = "Công việc cần được duyệt.",
                            CompletionRating = 5
                        };
                        if (await _projectTaskApproveRepo.CreateAsync(newProjectTaskApprove) <= 0)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, "Failed to create project task approve."));
                        }
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
                        Status = projectTask.Status ?? 0,
                        IsApproved = 1,
                        IsDeleted = false,
                        EmployeeIDRequest = projectTask.EmployeeIDRequest,
                        EmployeeCreateID = currentUser.EmployeeID,
                        PlanStartDate = projectTask.PlanStartDate,
                        PlanEndDate = projectTask.PlanEndDate.HasValue ? projectTask.PlanEndDate.Value.Date.AddDays(1).AddSeconds(-1) : null,
                        IsPersonalProject = projectTask.IsPersonalProject,
                        TypeProjectItem = projectTask.TypeProjectItem,
                        IsAdditional = projectTask.IsAdditional,
                        TaskComplexity = projectTask.TaskComplexity.HasValue && projectTask.TaskComplexity > 0 ? projectTask.TaskComplexity : 1,
                        ParentID = projectTask.ParentID,
                        Deadline = projectTask.Deadline,
                        DescriptionSolution = projectTask.DescriptionSolution,
                        ProjectTaskResult = projectTask.ProjectTaskResult,
                        ProjectTaskTypeID = projectTask.ProjectTaskTypeID.HasValue && projectTask.ProjectTaskTypeID > 0 ? projectTask.ProjectTaskTypeID : 1,
                        Priority = projectTask.Priority ?? 1,
                        EstimatedTime = projectTask.EstimatedTime == null || projectTask.EstimatedTime == 0 ? null : projectTask.EstimatedTime,
                        NeedApprove = projectTask.NeedApprove ?? true
                    };

                    if (projectTask.Employee != null && projectTask.Employee.Count > 0)
                    {
                        Employee userAssignee = await _employeeRepo.GetByIDAsync(projectTask.Employee[0]);
                        newProjectTask.UserID = userAssignee.UserID;
                    }

                    if (newProjectTask.UserID == null || newProjectTask.UserID < 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "User Is Null, please chose user!"));
                    }

                    // Tính % quá hạn
                    if (projectTask.Status == 2 && projectTask.ActualEndDate != null
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

                    // Tạo mã code cho công việc
                    if (projectTask.ProjectID != null && projectTask.ProjectID > 0)
                    {
                        newProjectTask.Code = _projectItemRepo.GenerateProjectItemCode(projectTask.ProjectID ?? 0).Trim();

                    }
                    else
                    {
                        //var employeeById = _employeeRepo.GetByID(currentUser.EmployeeID);
                        newProjectTask.Code = _projectTaskRepo.GenerateProjectTaskCodeTime(projectTask.ProjectTaskTypeID ?? 1).Trim();
                    }


                    // tạo mới công việc
                    if (await _projectItemRepo.CreateAsync(newProjectTask) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to create project task."));
                    }

                    // Gửi mail
                    var listEmail = new List<String>();
                    List<UserTeam> leaders = new List<UserTeam>();

                    // Thêm mới người thực hiện vào danh sách người thực hiện (ProjectTaskEmployee)(type = 1) + thêm vào danh sách gửi mail
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

                            // Bỏ người tạo công việc ra khỏi danh sách gửi mail
                            if (emp != currentUser.EmployeeID)
                            {
                                if (employeeValue != null && employeeValue.EmailCongTy != null)
                                    listEmail.Add(employeeValue.EmailCongTy);
                            }
                            if (await _projectTaskEmployeeRepo.CreateAsync(projectTaskEmployee) <= 0)
                            {
                                return BadRequest(ApiResponseFactory.Fail(null, "Failed to assign employee to project task."));
                            }
                        }
                    }



                    // Lấy danh sách người liên quan
                    if (projectTask.EmployeeRelate != null && projectTask.EmployeeRelate.Count > 0)
                    {

                        // Thêm người tạo công việc vào người liên quan nếu người đó chưa lằm trong danh sách người liên quan + không lằm trong người nhận việc + không phải là người giao việc 
                        if (!projectTask.EmployeeRelate.Contains(currentUser.EmployeeID) && (projectTask.Employee == null || !projectTask.Employee.Contains(currentUser.EmployeeID)) && projectTask.EmployeeIDRequest != currentUser.EmployeeID)
                        {
                            projectTask.EmployeeRelate.Add(currentUser.EmployeeID);
                        }
                    }

                    // Lấy danh sách leader trực tiếp của người nhận việc khi đó là công việc dự án
                    if (projectTask.IsPersonalProject != true)
                    {
                        string usersString = "";

                        if (projectTask.Employee != null && projectTask.Employee.Count > 0)
                        {
                            usersString = string.Join(",", projectTask.Employee);

                        }

                        var param = new
                        {
                            p_UserID = usersString
                        };
                        leaders = await SqlDapper<UserTeam>.ProcedureToListTAsync("spGetLeaderTeam", param);
                    }
                    // Thêm leader vào sách người liên quan
                    if (leaders.Count > 0)
                    {
                        foreach (var item in leaders)
                        {
                            // Nếu leader chưa lằm trong danh sách người liên quan + không lằm trong người nhận việc + không phải là người giao việc thì thêm vào danh sách người liên quan
                            if (item.LeaderID != null && (projectTask.EmployeeRelate == null || !projectTask.EmployeeRelate.Contains(item.LeaderID ?? -1)) && (projectTask.Employee == null || !projectTask.Employee.Contains(item.LeaderID ?? -1)) && projectTask.EmployeeIDRequest != item.LeaderID)
                            {
                                if (projectTask.EmployeeRelate == null)
                                {
                                    projectTask.EmployeeRelate = new List<int>();
                                }
                                projectTask.EmployeeRelate.Add(item.LeaderID ?? 0);
                            }
                        }
                    }

                    // Thêm mới người liên quan vào bảng ProjectTaskEmployee (type = 2) + thêm vào danh sách gửi mail
                    var listEmployeeRelate = new List<string>();
                    if (projectTask.EmployeeRelate != null && projectTask.EmployeeRelate.Count > 0)
                    {
                        foreach (var emp in projectTask.EmployeeRelate)
                        {
                            var employeeValue = await _employeeRepo.GetByIDAsync(emp);

                            // Thêm người liên quan vào danh sách gửi mail
                            if (employeeValue != null)
                            {
                                if (employeeValue != null && employeeValue.EmailCongTy != null)
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
                    }
                    var listEmailBand = _projectTaskSettingRepo.GetAll(x => x.SendMailCreateProjectTask == false).Select(x => x.EmployeeEmail);

                    var listemailRecive = listEmail.Except(listEmailBand);
                    var listemailCC = listEmployeeRelate.Except(listEmailBand);

                    string listToEmailString = string.Join(",", listemailRecive.Distinct());
                    string listEmployeeRelateString = string.Join(",", listemailCC.Distinct());
                    var employeeAsigner = await _employeeRepo.GetByIDAsync(newProjectTask.EmployeeIDRequest ?? 0);
                    if (listToEmailString.Count() > 0)
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

                    if(!(projectTask.NeedApprove ?? true) && projectTask.Status == 2)
                    {
                        var newProjectTaskApprove = new ProjectTaskApprove
                        {
                            ProjectTaskID = newProjectTask.ID,
                            IsApprove = true,
                            EmployeeID = currentUser.EmployeeID,
                            //Review = "Công việc cần được duyệt.",
                            CompletionRating = 5
                        };
                        if (await _projectTaskApproveRepo.CreateAsync(newProjectTaskApprove) <= 0)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, "Failed to create project task approve."));
                        }
                    }

                    return Ok(ApiResponseFactory.Success(newProjectTask));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to create project task."));
            }
        }

        [HttpPost("cancel-approve")]
        public IActionResult CancelApprove([FromBody] int projectTaskID)
        {
            try
            {
                if(projectTaskID == null || projectTaskID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Project task data is null."));
                }
                    
                var dataExit =  _projectTaskApproveRepo.GetAll(x => x.ProjectTaskID == projectTaskID).FirstOrDefault();
                if(dataExit == null || dataExit.ID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Can't fount project task approve."));
                }
                dataExit.IsDeleted = true;
                _projectTaskApproveRepo.UpdateAsync(dataExit);
                return Ok(ApiResponseFactory.Success(dataExit));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Failed to approve project task."));
            }
        }


        [HttpPost("Approve")]
        public async Task<IActionResult> ApproveTask([FromBody] List<int> projectTaskIDs, bool isApproved, string? review, int? completionRating)
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
                            var employeeValue = await _employeeRepo.GetByIDAsync(emp??0);

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
                        var employeeValue = await _employeeRepo.GetByIDAsync(emp??0);
                        if (employeeValue != null)
                        {
                            listEmployeeRelate2.Add(employeeValue.EmailCongTy);
                        }
                    }

                    var listEmailBand = _projectTaskSettingRepo.GetAll(x => x.SendApproveProjectTask == false).Select(x => x.EmployeeEmail);

                    var listemailRecive = listEmployeeRelate2.Except(listEmailBand);
                    var listemailCC = listEmail1.Except(listEmailBand);

                    string listEmployeeRelateString1 = string.Join(",", listemailRecive.Distinct());
                    string listToEmailString1 = string.Join(",", listemailCC.Distinct());
                    var employeeAsigner1 = await _employeeRepo.GetByIDAsync(exitProjectTask.EmployeeIDRequest ?? 0);


                    if (listToEmailString1.Count() > 0)
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
                    if (exitProjectTask.Status != 2)
                    {
                        continue; // chỉ duyệt những công việc chưa hoàn thành
                    }
                    var newApproveTask = new ProjectTaskApprove
                    {
                        ProjectTaskID = taskApprove,
                        IsApprove = isApproved,
                        Review = review,
                        EmployeeID = currentUser.EmployeeID,
                        CompletionRating = completionRating
                    };

                    if (await _projectTaskApproveRepo.CreateAsync(newApproveTask) <= 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Failed to approve project task."));
                    }

                    //if (isApproved)
                    //{
                    //    exitProjectTask.IsApproved = 2;
                    //}
                    //else
                    //{
                    //    exitProjectTask.IsApproved = 3;
                    //}

                    var dayActual = (exitProjectTask.ActualEndDate.Value.Date - exitProjectTask.ActualStartDate.Value.Date).TotalDays + 1;
                    var dayPlan = (exitProjectTask.PlanEndDate.Value.Date - exitProjectTask.PlanStartDate.Value.Date).TotalDays + 1;
                    if ((dayActual - dayPlan) > 0)
                    {
                        exitProjectTask.PercentOverTime = Math.Round((decimal)(dayActual - dayPlan) / (decimal)dayPlan, 2);
                    }
                    else
                    {
                        exitProjectTask.PercentOverTime = 0;
                    }


                    exitProjectTask.UpdatedDate = exitProjectTask.UpdatedDate;

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
                        Status = 0,
                        IsApproved = 1,
                        PlanStartDate = item.PlanStartDate,
                        PlanEndDate = item.PlanEndDate.HasValue ? item.PlanEndDate.Value.Date.AddDays(1).AddSeconds(-1) : (DateTime?)null,
                        EmployeeCreateID = currentUser.EmployeeID,
                        ProjectTaskTypeID = 1
                    };

                    if (employeeAssigneeIDs != null && employeeAssigneeIDs.Count > 0)
                    {
                        Employee userAssignee = await _employeeRepo.GetByIDAsync(employeeAssigneeIDs[0]);
                        createProjectTask.UserID = userAssignee.UserID;

                        if (createProjectTask.UserID == null || createProjectTask.UserID < 0)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, "User Is Null, please chose user!"));
                        }
                    }
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
