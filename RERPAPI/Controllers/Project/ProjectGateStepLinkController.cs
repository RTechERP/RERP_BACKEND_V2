using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectGateStepLinkController : ControllerBase
    {
        private readonly ProjectGateStepLinkRepo _stepLinkRepo;
        private readonly ProjectGateStepWorkerRepo _stepWorkerRepo;
        private readonly ProjectGateStepCheckListLinkRepo _stepCheckListLinkRepo;
        private readonly ProjectGateStepCheckListRepo _stepCheckListRepo;
        private readonly ProjectGateStepFileRepo _stepFileRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly ProjectTreeFolderRepo _treeFolderRepo;
        private readonly ProjectItemRepo _projectItemRepo;
        private readonly ProjectTaskEmployeeRepo _taskEmployeeRepo;
        private readonly CurrentUser _currentUser;

        public ProjectGateStepLinkController(
            ProjectGateStepLinkRepo stepLinkRepo,
            ProjectGateStepWorkerRepo stepWorkerRepo,
            ProjectGateStepCheckListLinkRepo stepCheckListLinkRepo,
            ProjectGateStepCheckListRepo stepCheckListRepo,
               ProjectGateStepFileRepo stepFileRepo,
            ProjectRepo projectRepo,
            ProjectTreeFolderRepo treeFolderRepo,
            ProjectItemRepo projectItemRepo,
            ProjectTaskEmployeeRepo taskEmployeeRepo,
            CurrentUser currentUser)
        {
            _stepLinkRepo = stepLinkRepo;
            _stepWorkerRepo = stepWorkerRepo;
            _stepFileRepo = stepFileRepo;
            _stepCheckListLinkRepo = stepCheckListLinkRepo;
            _stepCheckListRepo = stepCheckListRepo;
            _stepFileRepo = stepFileRepo;
            _projectRepo = projectRepo;
            _treeFolderRepo = treeFolderRepo;
            _projectItemRepo = projectItemRepo;
            _taskEmployeeRepo = taskEmployeeRepo;
            _currentUser = currentUser;
        }


        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] SaveProjectGateStepRequestDto request)
        {
            try
            {
                if (request == null || request.ProjectID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu không hợp lệ"));
                }

                // Ngăn chặn tràn số học khi lưu các trường decimal của ProjectItem
                if (request.Steps != null)
                {
                    foreach (var step in request.Steps)
                    {
                        if (step.DayCount.HasValue && step.DayCount.Value > 9999)
                        {
                            return BadRequest(ApiResponseFactory.Fail(null, "Số ngày (DayCount) vượt quá giới hạn cho phép. Vui lòng kiểm tra lại dữ liệu."));
                        }
                    }
                }

                // Lấy thông tin dự án để gen path thư mục
                var project = _projectRepo.GetByID(request.ProjectID);

                // Lấy toàn bộ các link hiện có của dự án này trong DB
                var existingLinks = _stepLinkRepo.GetAll(l => l.ProjectID == request.ProjectID).ToList();
                var processedLinkIDs = new List<int>();

                if (request.Steps != null && request.Steps.Any())
                {
                    foreach (var step in request.Steps)
                    {
                        // Tìm link tương ứng hiện có trong DB
                        var matchingLink = existingLinks.FirstOrDefault(l =>
                            l.ProjectTypeID == step.ProjectTypeID &&
                            l.ProjectGateStepID == step.ProjectGateStepID &&
                            l.IsRepeat == step.IsRepeat);

                        if (matchingLink != null)
                        {
                            // ── CẬP NHẬT LINK HIỆN CÓ ──
                            matchingLink.StartDate = step.StartDate;
                            matchingLink.IsDeleted = false; // Đảm bảo chưa bị xóa
                            await _stepLinkRepo.UpdateAsync(matchingLink);
                            processedLinkIDs.Add(matchingLink.ID);

                            // Đồng bộ nhân viên (Workers) của Link này
                            var existingWorkers = _stepWorkerRepo.GetAll(w => w.ProjectGateStepLinkID == matchingLink.ID).ToList();

                            // Xóa nhân viên không còn trong request
                            var workersToDelete = existingWorkers.Where(ew =>
                                step.Workers == null || !step.Workers.Any(sw => sw.EmployeeID == ew.EmployeeID)).ToList();
                            foreach (var ew in workersToDelete)
                            {
                                await _stepWorkerRepo.DeleteAsync(ew.ID);
                            }

                            // Cập nhật hoặc Thêm mới nhân viên
                            if (step.Workers != null)
                            {
                                foreach (var w in step.Workers)
                                {
                                    var matchingWorker = existingWorkers.FirstOrDefault(ew => ew.EmployeeID == w.EmployeeID);
                                    if (matchingWorker != null)
                                    {
                                        matchingWorker.DayCount = w.DayCount;
                                        matchingWorker.UnitPrice = w.UnitPrice;
                                        matchingWorker.TotalAmount = w.TotalAmount;
                                        await _stepWorkerRepo.UpdateAsync(matchingWorker);
                                    }
                                    else
                                    {
                                        var newWorker = new ProjectGateStepWorker
                                        {
                                            ProjectGateStepLinkID = matchingLink.ID,
                                            EmployeeID = w.EmployeeID,
                                            DayCount = w.DayCount,
                                            UnitPrice = w.UnitPrice,
                                            TotalAmount = w.TotalAmount
                                        };
                                        await _stepWorkerRepo.CreateAsync(newWorker);
                                    }
                                }
                            }

                            // Đồng bộ hoặc tạo mới ProjectItem công việc
                            if (step.StartDate.HasValue
                                && step.DayCount.HasValue && step.DayCount.Value > 0
                                && step.Workers != null && step.Workers.Any())
                            {
                                string mission = !string.IsNullOrEmpty(step.Content) ? step.Content : $"Gate Step #{step.ProjectGateStepID}";
                                var planStart = step.StartDate.Value;
                                var planEnd = planStart.AddDays((double)step.DayCount.Value);

                                if (matchingLink.ProjectTaskID.HasValue)
                                {
                                    var projectItem = _projectItemRepo.GetByID(matchingLink.ProjectTaskID.Value);
                                    if (projectItem != null)
                                    {
                                        projectItem.Mission = mission;
                                        projectItem.PlanStartDate = planStart;
                                        projectItem.PlanEndDate = planEnd;
                                        projectItem.Deadline = planEnd;
                                        projectItem.EstimatedTime = (int)step.DayCount.Value;
                                        projectItem.UserID = step.Workers.First().EmployeeID;
                                        projectItem.IsDeleted = false;
                                        projectItem.EmployeeIDRequest = _currentUser.EmployeeID;
                                        _projectItemRepo.CalculateDays(projectItem);
                                        await _projectItemRepo.UpdateAsync(projectItem);

                                        await SyncProjectTaskEmployees(projectItem.ID, step.Workers);
                                    }
                                }
                                else
                                {
                                    var newItem = new ProjectItem
                                    {
                                        ProjectID = request.ProjectID,
                                        Mission = mission,
                                        PlanStartDate = planStart,
                                        PlanEndDate = planEnd,
                                        Deadline = planEnd,
                                        Status = 0,
                                        ItemLate = 0,
                                        Code = _projectItemRepo.GenerateProjectItemCode(request.ProjectID),
                                        EstimatedTime = (int)step.DayCount.Value,
                                        UserID = step.Workers.First().EmployeeID,
                                        STT = _projectItemRepo.GetMaxSTT(request.ProjectID),
                                        IsDeleted = false,
                                        EmployeeIDRequest = _currentUser.EmployeeID
                                    };
                                    _projectItemRepo.CalculateDays(newItem);
                                    if (await _projectItemRepo.CreateAsync(newItem) > 0)
                                    {
                                        matchingLink.ProjectTaskID = newItem.ID;
                                        await _stepLinkRepo.UpdateAsync(matchingLink);

                                        await SyncProjectTaskEmployees(newItem.ID, step.Workers);
                                    }
                                }
                            }

                            // Đồng bộ checklist links
                            await SyncCheckListLinksAsync(matchingLink.ID, step.ProjectGateStepID, step.ProjectTypeID, project);
                        }
                        else
                        {
                            // ── TẠO MỚI LINK CHƯA CÓ ──
                            var newLink = new ProjectGateStepLink
                            {
                                ProjectID = request.ProjectID,
                                ProjectGateStepID = step.ProjectGateStepID,
                                ProjectTypeID = step.ProjectTypeID,
                                StartDate = step.StartDate,
                                IsRepeat = step.IsRepeat,
                                IsDeleted = false
                            };

                            await _stepLinkRepo.CreateAsync(newLink);

                            if (newLink.ID > 0)
                            {
                                processedLinkIDs.Add(newLink.ID);

                                // Thêm người thực hiện
                                if (step.Workers != null && step.Workers.Any())
                                {
                                    var newWorkers = step.Workers.Select(w => new ProjectGateStepWorker
                                    {
                                        ProjectGateStepLinkID = newLink.ID,
                                        EmployeeID = w.EmployeeID,
                                        DayCount = w.DayCount,
                                        UnitPrice = w.UnitPrice,
                                        TotalAmount = w.TotalAmount
                                    }).ToList();

                                    await _stepWorkerRepo.CreateRangeAsync(newWorkers);
                                }

                                // Tạo ProjectItem công việc
                                if (step.StartDate.HasValue
                                    && step.DayCount.HasValue && step.DayCount.Value > 0
                                    && step.Workers != null && step.Workers.Any())
                                {
                                    string mission = !string.IsNullOrEmpty(step.Content) ? step.Content : $"Gate Step #{step.ProjectGateStepID}";
                                    var planStart = step.StartDate.Value;
                                    var planEnd = planStart.AddDays((double)step.DayCount.Value);

                                    var newItem = new ProjectItem
                                    {
                                        ProjectID = request.ProjectID,
                                        Mission = mission,
                                        PlanStartDate = planStart,
                                        PlanEndDate = planEnd,
                                        Status = 0,
                                        ItemLate = 0,
                                        NeedApprove = true,
                                        Priority = 1,
                                        Deadline = planEnd,
                                        Code = _projectItemRepo.GenerateProjectItemCode(request.ProjectID),
                                        EstimatedTime = (int)step.DayCount.Value,
                                        UserID = step.Workers.First().EmployeeID,
                                        STT = _projectItemRepo.GetMaxSTT(request.ProjectID),
                                        IsDeleted = false,
                                        EmployeeIDRequest = _currentUser.EmployeeID
                                    };
                                    _projectItemRepo.CalculateDays(newItem);
                                    if (await _projectItemRepo.CreateAsync(newItem) > 0)
                                    {
                                        newLink.ProjectTaskID = newItem.ID;
                                        await _stepLinkRepo.UpdateAsync(newLink);

                                        await SyncProjectTaskEmployees(newItem.ID, step.Workers);
                                    }
                                }

                                // Tạo Checklist link
                                var checkListDefs = _stepCheckListRepo.GetAll(c => c.ProjectGateStepID == step.ProjectGateStepID).ToList();
                                if (checkListDefs.Any())
                                {
                                    var newCheckLists = new List<ProjectGateStepCheckListLink>();
                                    foreach (var def in checkListDefs)
                                    {
                                        if (def.Type != "FilePath" && def.Type != "File_Path") continue;

                                        string pathFolder = "";
                                        if (project != null && project.CreatedDate.HasValue)
                                        {
                                            int year = project.CreatedDate.Value.Year;
                                            var rootFolder = _treeFolderRepo.GetAll(f => f.ProjectTypeID == step.ProjectTypeID
                                                                                      && (f.ParentID == 0 || f.ParentID == null)
                                                                                      && (f.IsDeleted == null || f.IsDeleted == false)).FirstOrDefault();

                                            string typeFolderName = rootFolder?.FolderName ?? "TaiLieuChung";
                                            string folderName = !string.IsNullOrEmpty(def.Description) ? def.Description.Trim() : $"Step_{step.ProjectGateStepID}_CheckList_{def.ID}";

                                            pathFolder = Path.Combine(@"\\192.168.1.190\duan", "projects", year.ToString(), project.ProjectCode, typeFolderName, folderName);

                                            try
                                            {
                                                if (!Directory.Exists(pathFolder))
                                                {
                                                    Directory.CreateDirectory(pathFolder);
                                                }
                                            }
                                            catch { }
                                        }

                                        newCheckLists.Add(new ProjectGateStepCheckListLink
                                        {
                                            ProjectGateStepLinkID = newLink.ID,
                                            ProjectGateStepCheckListID = def.ID,
                                            PathFolder = pathFolder,
                                            IsPass = false
                                        });
                                    }

                                    if (newCheckLists.Any())
                                    {
                                        await _stepCheckListLinkRepo.CreateRangeAsync(newCheckLists);
                                    }
                                }
                            }
                        }
                    }
                }

                // ── XÓA MỀM CÁC LINK KHÔNG CÓ TRONG REQUEST ──
                var deletedLinks = existingLinks.Where(l => !processedLinkIDs.Contains(l.ID) && (l.IsDeleted == null || l.IsDeleted == false)).ToList();
                foreach (var dl in deletedLinks)
                {
                    dl.IsDeleted = true;
                    await _stepLinkRepo.UpdateAsync(dl);

                    // Xóa mềm ProjectItem đi kèm
                    if (dl.ProjectTaskID.HasValue)
                    {
                        var pi = _projectItemRepo.GetByID(dl.ProjectTaskID.Value);
                        if (pi != null)
                        {
                            pi.IsDeleted = true;
                            await _projectItemRepo.UpdateAsync(pi);

                            var taskEmps = _taskEmployeeRepo.GetAll(x => x.ProjectTaskID == pi.ID && x.IsDeleted != true).ToList();
                            foreach (var te in taskEmps)
                            {
                                te.IsDeleted = true;
                                te.UpdatedDate = DateTime.Now;
                                te.UpdatedBy = _currentUser.FullName;
                                await _taskEmployeeRepo.UpdateAsync(te);
                            }
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(true, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        private async Task SyncProjectTaskEmployees(int projectTaskId, List<ProjectGateStepWorkerDto> workers)
        {
            var existing = _taskEmployeeRepo.GetAll(x => x.ProjectTaskID == projectTaskId && x.IsDeleted != true).ToList();

            // Xóa các nhân viên không còn nằm trong danh sách
            foreach (var ext in existing)
            {
                if (workers == null || !workers.Any(w => w.EmployeeID == ext.EmployeeID))
                {
                    ext.IsDeleted = true;
                    await _taskEmployeeRepo.UpdateAsync(ext);
                }
            }

            // Thêm nhân viên mới hoặc kích hoạt lại các nhân viên đã bị xóa mềm
            if (workers != null)
            {
                foreach (var w in workers)
                {
                    var ext = existing.FirstOrDefault(x => x.EmployeeID == w.EmployeeID);
                    if (ext == null)
                    {
                        var deletedEmp = _taskEmployeeRepo.GetAll(x => x.ProjectTaskID == projectTaskId && x.EmployeeID == w.EmployeeID && x.IsDeleted == true).FirstOrDefault();
                        if (deletedEmp != null)
                        {
                            deletedEmp.IsDeleted = false;
                            deletedEmp.Type = 1; // 1: người nhận việc
                            await _taskEmployeeRepo.UpdateAsync(deletedEmp);
                        }
                        else
                        {
                            var newEmp = new ProjectTaskEmployee
                            {
                                ProjectTaskID = projectTaskId,
                                EmployeeID = w.EmployeeID,
                                Type = 1, // 1: người nhận việc
                                IsDeleted = false
                            };
                            await _taskEmployeeRepo.CreateAsync(newEmp);
                        }
                    }
                }
            }
        }
        [HttpGet("GetByProject/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            try
            {
                // SP trả về 3 result sets: links, workers, checklists+files
                var (links, workers, checklistFileRows) = await SqlDapper<ProjectGateStepLinkResultDto>
                    .QueryMultipleAsync<ProjectGateStepLinkResultDto, ProjectGateStepWorkerResultDto, ProjectGateStepCheckListFileResultDto>(
                        "spGetProjectGateStepLinkByProject",
                        new { ProjectID = projectId }
                    );

                var workerGroups = workers
                    .GroupBy(w => w.ProjectGateStepLinkID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Gom checklist theo LinkID, sau đó gom file theo CheckListLinkID
                var checklistByLink = checklistFileRows
                    .GroupBy(r => r.ProjectGateStepLinkID)
                    .ToDictionary(g => g.Key, g =>
                        g.GroupBy(r => r.CheckListLinkID)
                         .Select(clGroup =>
                         {
                             var first = clGroup.First();
                             return new ProjectGateStepCheckListLinkDto
                             {
                                 ID = first.CheckListLinkID,
                                 ProjectGateStepCheckListID = first.ProjectGateStepCheckListID,
                                 PathFolder = first.PathFolder,
                                 IsPass = first.IsPass,
                                 IsRequired = first.IsRequired,
                                 Description = first.Description,
                                 Type = first.Type,
                                 Files = clGroup
                                     .Where(r => r.FileID.HasValue)
                                     .Select(r => new ProjectGateStepFileDto
                                     {
                                         ID = r.FileID!.Value,
                                         FileName = r.FileName!,
                                         FilePath = r.FilePath!,
                                         FileSize = r.FileSize,
                                         ContentType = r.ContentType,
                                         CreatedBy = r.CreatedBy,
                                         CreatedDate = r.CreatedDate,
                                     }).ToList()
                             };
                         }).ToList()
                    );

                var result = links.Select(l => new ProjectGateStepLinkDto
                {
                    ProjectGateStepID = l.ProjectGateStepID,
                    ProjectTypeID = l.ProjectTypeID,
                    StartDate = l.StartDate,
                    IsRepeat = l.IsRepeat,
                    IsApproved = l.IsApproved,
                    ApprovedBy = l.ApprovedBy,
                    ApprovedDate = l.ApprovedDate,
                    ApprovalComment = l.ApprovalComment,
                    Workers = workerGroups.TryGetValue(l.ID, out var wList)
                        ? wList.Select(w => new ProjectGateStepWorkerDto
                        {
                            EmployeeID = w.EmployeeID,
                            DayCount = w.DayCount,
                            UnitPrice = w.UnitPrice,
                            TotalAmount = w.TotalAmount
                        }).ToList()
                        : new List<ProjectGateStepWorkerDto>(),
                    CheckLists = checklistByLink.TryGetValue(l.ID, out var clDtos)
                        ? clDtos
                        : new List<ProjectGateStepCheckListLinkDto>()
                }).ToList();

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("GetDeletedByProject/{projectId}")]
        public async Task<IActionResult> GetDeletedByProject(int projectId)
        {
            try
            {
                var (links, workers) = await SqlDapper<ProjectGateStepLinkResultDto>
                    .QueryMultipleAsync<ProjectGateStepLinkResultDto, ProjectGateStepWorkerResultDto>(
                        "spGetProjectGateStepLinkDeletedByProject",
                        new { ProjectID = projectId }
                    );

                var workerGroups = workers
                    .GroupBy(w => w.ProjectGateStepLinkID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var allLinkIds = links.Select(x => x.ID).ToList();
                var allChecklists = _stepCheckListLinkRepo.GetAll(c => allLinkIds.Contains(c.ProjectGateStepLinkID ?? 0)).ToList();
                var allCheckListIds = allChecklists.Select(c => c.ID).ToList();
                var allFiles = _stepFileRepo.GetAll(f => allCheckListIds.Contains(f.ProjectGateStepCheckListLinkID) && (f.IsDeleted == false || f.IsDeleted == null)).ToList();
                var filesByCheckList = allFiles.GroupBy(f => f.ProjectGateStepCheckListLinkID).ToDictionary(g => g.Key, g => g.ToList());
                var checklistGroups = allChecklists.GroupBy(c => c.ProjectGateStepLinkID).ToDictionary(g => g.Key, g => g.ToList());

                var result = links.Select(l => new ProjectGateStepLinkDto
                {
                    ID = l.ID,
                    ProjectGateStepID = l.ProjectGateStepID,
                    ProjectTypeID = l.ProjectTypeID,
                    StartDate = l.StartDate,
                    IsRepeat = l.IsRepeat,
                    IsApproved = l.IsApproved,
                    ApprovedBy = l.ApprovedBy,
                    ApprovedDate = l.ApprovedDate,
                    ApprovalComment = l.ApprovalComment,
                    Workers = workerGroups.TryGetValue(l.ID, out var wList)
                        ? wList.Select(w => new ProjectGateStepWorkerDto
                        {
                            EmployeeID = w.EmployeeID,
                            DayCount = w.DayCount,
                            UnitPrice = w.UnitPrice,
                            TotalAmount = w.TotalAmount
                        }).ToList()
                        : new List<ProjectGateStepWorkerDto>(),
                    CheckLists = checklistGroups.TryGetValue(l.ID, out var cList)
                        ? cList.Select(c => new ProjectGateStepCheckListLinkDto
                        {
                            ID = c.ID,
                            ProjectGateStepCheckListID = c.ProjectGateStepCheckListID,
                            PathFolder = c.PathFolder,
                            IsPass = c.IsPass,
                            Files = filesByCheckList.TryGetValue(c.ID, out var fList)
                                ? fList.Select(f => new ProjectGateStepFileDto
                                {
                                    ID = f.ID,
                                    FileName = f.FileName,
                                    FilePath = f.FilePath,
                                    FileSize = f.FileSize,
                                    ContentType = f.ContentType,
                                    CreatedBy = f.CreatedBy,
                                    CreatedDate = f.CreatedDate
                                }).ToList()
                                : new List<ProjectGateStepFileDto>()
                        }).ToList()
                        : new List<ProjectGateStepCheckListLinkDto>()
                }).ToList();

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // ── ENDPOINT: Lưu thông tin file sau khi upload lên server ──
        [HttpPost("SaveFile/{checkListLinkId}")]
        public async Task<IActionResult> SaveFile(int checkListLinkId, [FromBody] ProjectGateStepFileDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.FileName) || string.IsNullOrWhiteSpace(dto.FilePath))
                    return BadRequest(ApiResponseFactory.Fail(null, "Thông tin file không hợp lệ"));

                var checkListLink = _stepCheckListLinkRepo.GetByID(checkListLinkId);
                if (checkListLink == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy CheckListLink"));

                var newFile = new ProjectGateStepFile
                {
                    ProjectGateStepCheckListLinkID = checkListLinkId,
                    FileName = dto.FileName,
                    FilePath = dto.FilePath,
                    FileSize = dto.FileSize,
                    ContentType = dto.ContentType,
                    IsDeleted = false,
                    CreatedBy = User.Identity?.Name ?? "System",
                    CreatedDate = DateTime.Now
                };

                await _stepFileRepo.CreateAsync(newFile);
                return Ok(ApiResponseFactory.Success(newFile.ID, "Lưu thông tin file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("GetFiles/{checkListLinkId}")]
        public IActionResult GetFiles(int checkListLinkId)
        {
            try
            {
                var files = _stepFileRepo.GetAll(f => f.ProjectGateStepCheckListLinkID == checkListLinkId && (f.IsDeleted == false || f.IsDeleted == null))
                    .Select(f => new ProjectGateStepFileDto
                    {
                        ID = f.ID,
                        FileName = f.FileName,
                        FilePath = f.FilePath,
                        FileSize = f.FileSize,
                        ContentType = f.ContentType,
                        CreatedBy = f.CreatedBy,
                        CreatedDate = f.CreatedDate
                    }).ToList();

                return Ok(ApiResponseFactory.Success(files, "Lấy danh sách file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // ── ENDPOINT: Xóa mềm một file ──
        [HttpDelete("DeleteFile/{fileId}")]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            try
            {
                var file = _stepFileRepo.GetByID(fileId);
                if (file == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy file"));

                file.IsDeleted = true;
                file.UpdatedBy = User.Identity?.Name ?? "System";
                file.UpdatedDate = DateTime.Now;
                await _stepFileRepo.UpdateAsync(file);

                return Ok(ApiResponseFactory.Success(true, "Xóa file thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // ── ENDPOINT: TBP Duyệt công đoạn ──
        [HttpPost("Approve/{linkId}")]
        public async Task<IActionResult> Approve(int linkId, [FromBody] string? comment)
        {
            try
            {
                var link = _stepLinkRepo.GetByID(linkId);
                if (link == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy bước công việc"));

                // 1. Sử dụng Dapper để query kiểm tra xem có bất kỳ checklist bắt buộc nào chưa pass không
                using (var connection = new SqlConnection(Config.ConnectionString))
                {
                    string sql = @"
                        SELECT COUNT(1) 
                        FROM ProjectGateStepLink l
                        INNER JOIN ProjectGateStepCheckList c ON c.ProjectGateStepID = l.ProjectGateStepID
                            AND (c.ProjectTypeID IS NULL OR c.ProjectTypeID = l.ProjectTypeID)
                            AND (c.DepartmentID IS NULL OR c.DepartmentID IN (
                                SELECT pgd.DepartmentID 
                                FROM ProjectGateDepartment pgd 
                                WHERE pgd.ProjectGateStepID = l.ProjectGateStepID
                            ))
                        LEFT JOIN ProjectGateStepCheckListLink cl 
                               ON cl.ProjectGateStepLinkID = l.ID 
                              AND cl.ProjectGateStepCheckListID = c.ID
                        WHERE l.ID = @LinkID
                          AND c.IsRequired = 1
                          AND CAST(
                              CASE 
                                  WHEN c.Type IN (N'File_Path', N'File') THEN 
                                      CASE WHEN EXISTS (
                                          SELECT 1 
                                          FROM ProjectGateStepFile f2 
                                          WHERE f2.ProjectGateStepCheckListLinkID = cl.ID 
                                            AND (f2.IsDeleted = 0 OR f2.IsDeleted IS NULL)
                                      ) THEN 1 ELSE 0 END
                                  WHEN c.Type IN (N'Part_list', N'PartList') THEN 
                                      CASE WHEN EXISTS (
                                          SELECT 1 
                                          FROM ProjectPartList ppl 
                                          WHERE ppl.ProjectID = l.ProjectID 
                                            AND (ppl.IsDeleted = 0 OR ppl.IsDeleted IS NULL)
                                      ) THEN 1 ELSE 0 END
                                  ELSE ISNULL(cl.IsPass, 0)
                              END AS BIT) = 0";

                    int failedRequiredChecklists = await connection.ExecuteScalarAsync<int>(sql, new { LinkID = linkId });
                    if (failedRequiredChecklists > 0)
                    {
                        return BadRequest(ApiResponseFactory.Fail(null, "Không thể phê duyệt: Còn đầu mục checklist bắt buộc chưa hoàn thành."));
                    }
                }

                // 2. Thực hiện duyệt
                link.IsApproved = true;
                link.ApprovedBy = User.Identity?.Name ?? "TBP";
                link.ApprovedDate = DateTime.Now;
                link.ApprovalComment = comment;

                await _stepLinkRepo.UpdateAsync(link);

                return Ok(ApiResponseFactory.Success(true, "Phê duyệt công đoạn thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // ── ENDPOINT: TBP Từ chối công đoạn ──
        [HttpPost("Reject/{linkId}")]
        public async Task<IActionResult> Reject(int linkId, [FromBody] string? comment)
        {
            try
            {
                var link = _stepLinkRepo.GetByID(linkId);
                if (link == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy bước công việc"));

                link.IsApproved = false;
                link.ApprovedBy = User.Identity?.Name ?? "TBP";
                link.ApprovedDate = DateTime.Now;
                link.ApprovalComment = comment;

                await _stepLinkRepo.UpdateAsync(link);

                return Ok(ApiResponseFactory.Success(true, "Đã từ chối phê duyệt công đoạn"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // ── HELPER: Đồng bộ Checklist Links cho một công đoạn ──
        private async Task SyncCheckListLinksAsync(int stepLinkId, int gateStepId, int projectTypeId, RERPAPI.Model.Entities.Project project)
        {
            // 1. Lấy danh sách định nghĩa checklist cho step + project type + department
            List<ProjectGateStepCheckList> checkListDefs;
            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                string sql = @"
                    SELECT c.* 
                    FROM ProjectGateStepCheckList c
                    WHERE c.ProjectGateStepID = @ProjectGateStepID
                      AND (c.ProjectTypeID IS NULL OR c.ProjectTypeID = @ProjectTypeID)
                      AND (c.DepartmentID IS NULL OR c.DepartmentID IN (
                          SELECT pgd.DepartmentID 
                          FROM ProjectGateDepartment pgd 
                          WHERE pgd.ProjectGateStepID = @ProjectGateStepID
                      ))";
                checkListDefs = (await connection.QueryAsync<ProjectGateStepCheckList>(sql, new
                {
                    ProjectGateStepID = gateStepId,
                    ProjectTypeID = projectTypeId
                })).ToList();
            }

            // 2. Lấy checklist links hiện có
            var existingCheckListLinks = _stepCheckListLinkRepo.GetAll(cl => cl.ProjectGateStepLinkID == stepLinkId).ToList();

            // 3. Tạo các checklist link còn thiếu
            var newCheckLists = new List<ProjectGateStepCheckListLink>();
            foreach (var def in checkListDefs)
            {
                if (def.Type != "FilePath" && def.Type != "File_Path" && def.Type != "File") continue;

                if (!existingCheckListLinks.Any(cl => cl.ProjectGateStepCheckListID == def.ID))
                {
                    string pathFolder = "";
                    if (project != null && project.CreatedDate.HasValue)
                    {
                        int year = project.CreatedDate.Value.Year;
                        var rootFolder = _treeFolderRepo.GetAll(f => f.ProjectTypeID == projectTypeId
                                                                  && (f.ParentID == 0 || f.ParentID == null)
                                                                  && (f.IsDeleted == null || f.IsDeleted == false)).FirstOrDefault();

                        string typeFolderName = rootFolder?.FolderName ?? "TaiLieuChung";
                        string folderName = !string.IsNullOrEmpty(def.Description) ? def.Description.Trim() : $"Step_{gateStepId}_CheckList_{def.ID}";

                        pathFolder = Path.Combine(@"\\192.168.1.190\duan", "projects", year.ToString(), project.ProjectCode, typeFolderName, folderName);

                        try
                        {
                            if (!Directory.Exists(pathFolder))
                            {
                                Directory.CreateDirectory(pathFolder);
                            }
                        }
                        catch { }
                    }

                    newCheckLists.Add(new ProjectGateStepCheckListLink
                    {
                        ProjectGateStepLinkID = stepLinkId,
                        ProjectGateStepCheckListID = def.ID,
                        PathFolder = pathFolder,
                        IsPass = false
                    });
                }
            }

            if (newCheckLists.Any())
            {
                await _stepCheckListLinkRepo.CreateRangeAsync(newCheckLists);
            }
        }
    }
}
