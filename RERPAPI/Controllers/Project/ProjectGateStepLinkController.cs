
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RERPAPI.Attributes;
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

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectGateStepLinkController : ControllerBase
    {
        private readonly ProjectGateStepLinkRepo _stepLinkRepo;
        private readonly ProjectGateStepWorkerRepo _stepWorkerRepo;
        private readonly ProjectGateStepFileRepo _stepFileRepo;
        private readonly ProjectRepo _projectRepo;
        private readonly ProjectTreeFolderRepo _treeFolderRepo;
        private readonly ProjectItemRepo _projectItemRepo;
        private readonly ProjectTaskEmployeeRepo _taskEmployeeRepo;
        private readonly CurrentUser _currentUser;
        private readonly ProjectGateStepCheckListDetailLinkRepo _stepCheckListDetailLinkRepo;
        private readonly ProjectGateStepCheckListDetailRepo _stepCheckListDetailRepo;
        private readonly ProjectGateStepCheckListLinkRepo _stepCheckListLinkRepo;
        private readonly ProjectGateStepRepo _stepRepo;
        private readonly ProjectGateRepo _gateRepo;
        private readonly ProjectTypeLinkRepo _projectTypeLinkRepo;

        public ProjectGateStepLinkController(
            ProjectGateStepLinkRepo stepLinkRepo,
            ProjectGateStepWorkerRepo stepWorkerRepo,
            ProjectGateStepFileRepo stepFileRepo,
            ProjectRepo projectRepo,
            ProjectTreeFolderRepo treeFolderRepo,
            ProjectItemRepo projectItemRepo,
            ProjectTaskEmployeeRepo taskEmployeeRepo,
            CurrentUser currentUser,
            ProjectGateStepCheckListDetailLinkRepo stepCheckListDetailLinkRepo,
            ProjectGateStepCheckListDetailRepo stepCheckListDetailRepo,
            ProjectGateStepCheckListLinkRepo stepCheckListLinkRepo,
            ProjectGateStepRepo stepRepo,
            ProjectGateRepo gateRepo,
          
            ProjectTypeLinkRepo projectTypeLinkRepo)
        {
            _stepLinkRepo = stepLinkRepo;
            _stepWorkerRepo = stepWorkerRepo;
            _stepFileRepo = stepFileRepo;
            _projectRepo = projectRepo;
            _treeFolderRepo = treeFolderRepo;
            _projectItemRepo = projectItemRepo;
            _taskEmployeeRepo = taskEmployeeRepo;
            _currentUser = currentUser;
            _stepCheckListDetailLinkRepo = stepCheckListDetailLinkRepo;
            _stepCheckListDetailRepo = stepCheckListDetailRepo;
            _stepCheckListLinkRepo = stepCheckListLinkRepo;
            _stepRepo = stepRepo;
            _gateRepo = gateRepo;
            _projectTypeLinkRepo = projectTypeLinkRepo;
        }
        [RequiresPermission("N109")]
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
                var idMapping = new Dictionary<int, int>();

                if (request.Steps != null && request.Steps.Any())
                {
                    // Sắp xếp các bước: Các bước cha (ParentID == null hoặc 0) xử lý trước, bước con xử lý sau
                    var sortedSteps = request.Steps
                        .OrderBy(s => (s.ParentID.HasValue && s.ParentID.Value != 0) ? 1 : 0)
                        .ToList();

                    // Đảm bảo các kiểu dự án (ProjectTypeLink) trong danh sách bước được liên kết và lưu với dự án
                    var uniqueTypeIds = sortedSteps
                        .Where(s => s.ProjectTypeID > 0)
                        .Select(s => s.ProjectTypeID)
                        .Distinct()
                        .ToList();

                    foreach (var typeId in uniqueTypeIds)
                    {
                        var existingTypeLink = _projectTypeLinkRepo.GetAll(ptl => ptl.ProjectID == request.ProjectID && ptl.ProjectTypeID == typeId).FirstOrDefault();
                        if (existingTypeLink == null)
                        {
                            var newTypeLink = new ProjectTypeLink
                            {
                                ProjectID = request.ProjectID,
                                ProjectTypeID = typeId,
                                Selected = true,
                                CreatedDate = DateTime.Now,
                                CreatedBy = _currentUser?.LoginName
                            };
                            await _projectTypeLinkRepo.CreateAsync(newTypeLink);
                        }
                        else if (existingTypeLink.Selected != true)
                        {
                            existingTypeLink.Selected = true;
                            existingTypeLink.UpdatedDate = DateTime.Now;
                            existingTypeLink.UpdatedBy = _currentUser?.LoginName;
                            await _projectTypeLinkRepo.UpdateAsync(existingTypeLink);
                        }
                    }

                    foreach (var step in sortedSteps)
                    {
                        // Xác định realParentID: Nếu ParentID trỏ tới ID bản nháp tạm thời (số âm), tra cứu ID thật từ idMapping
                        int? realParentID = step.ParentID;
                        if (step.ParentID.HasValue && idMapping.ContainsKey(step.ParentID.Value))
                        {
                            realParentID = idMapping[step.ParentID.Value];
                        }
                        else if (step.ParentID.HasValue && step.ParentID.Value <= 0)
                        {
                            realParentID = null;
                        }

                        string gateCode = "";
                        var gateStep = _stepRepo.GetByID(step.ProjectGateStepID);
                        if (gateStep != null && gateStep.ProjectGateID.HasValue)
                        {
                            var gate = _gateRepo.GetByID(gateStep.ProjectGateID.Value);
                            if (gate != null && !string.IsNullOrEmpty(gate.GateCode))
                            {
                                gateCode = gate.GateCode;
                            }
                        }

                        // Tìm link tương ứng hiện có trong DB
                        ProjectGateStepLink matchingLink = null;
                        if (step.ID > 0)
                        {
                            matchingLink = existingLinks.FirstOrDefault(l => l.ID == step.ID);
                        }
                        else
                        {
                            matchingLink = existingLinks.FirstOrDefault(l =>
                                !processedLinkIDs.Contains(l.ID) &&
                                l.ProjectTypeID == step.ProjectTypeID &&
                                l.ProjectGateStepID == step.ProjectGateStepID &&
                                l.IsRepeat == step.IsRepeat &&
                                l.DepartmentID == step.DepartmentID &&
                                ((l.ParentID == null && realParentID == null) || (l.ParentID == realParentID)));
                        }

                        // Xác định parentProjectTaskId: Lấy ProjectTaskID của công đoạn cha (ProjectGateStepLink cha)
                        int? parentProjectTaskId = null;
                        if (realParentID.HasValue && realParentID.Value > 0)
                        {
                            var parentGateLink = existingLinks.FirstOrDefault(l => l.ID == realParentID.Value);
                            if (parentGateLink != null && parentGateLink.ProjectTaskID.HasValue && parentGateLink.ProjectTaskID.Value > 0)
                            {
                                parentProjectTaskId = parentGateLink.ProjectTaskID.Value;
                            }
                        }

                        if (matchingLink != null)
                        {
                            // ── CẬP NHẬT LINK HIỆN CÓ ──
                            matchingLink.StartDate = step.StartDate;
                            matchingLink.DateEnd = step.DateEnd;
                            matchingLink.IsDeleted = false; // Đảm bảo chưa bị xóa
                            matchingLink.ProjectGateStepTemplateID = step.ProjectGateStepTemplateID;
                            matchingLink.DepartmentID = step.DepartmentID;
                            matchingLink.ParentID = realParentID;
                            matchingLink.Content = step.Content;
                            matchingLink.ActualContent = step.ActualContent;
                            if (step.ProjectTypeID > 0)
                            {
                                matchingLink.ProjectTypeID = step.ProjectTypeID;
                            }
                            await _stepLinkRepo.UpdateAsync(matchingLink);
                            processedLinkIDs.Add(matchingLink.ID);
                            idMapping[step.ID] = matchingLink.ID;

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
                                string taskDescription = !string.IsNullOrEmpty(step.ActualContent) ? step.ActualContent : step.Content;
                                var planStart = step.StartDate.Value;
                                var planEnd = step.DateEnd.HasValue
                                    ? step.DateEnd.Value
                                    : (step.DayCount.Value > 0 ? planStart.AddDays((double)step.DayCount.Value - 1) : planStart);

                                if (matchingLink.ProjectTaskID.HasValue)
                                {
                                    var projectItem = _projectItemRepo.GetByID(matchingLink.ProjectTaskID.Value);
                                    if (projectItem != null)
                                    {
                                        projectItem.Mission = mission;
                                        projectItem.Description = taskDescription;
                                        projectItem.PlanStartDate = planStart;
                                        projectItem.PlanEndDate = planEnd;
                                        projectItem.Deadline = planEnd;
                                        projectItem.EstimatedTime = (int)(step.DayCount.Value * 8);
                                        projectItem.UserID = step.Workers.First().EmployeeID;
                                        projectItem.IsDeleted = false;
                                        projectItem.EmployeeIDRequest = _currentUser.EmployeeID;
                                        projectItem.TypeProjectItem = step.ProjectTypeID;
                                        projectItem.ParentID = parentProjectTaskId;
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
                                        TypeProjectItem = step.ProjectTypeID,
                                        Mission = mission,
                                        Description = taskDescription,
                                        PlanStartDate = planStart,
                                        PlanEndDate = planEnd,
                                        Deadline = planEnd,
                                        Status = 0,
                                        ItemLate = 0,
                                        Code = _projectItemRepo.GenerateProjectItemCodeByGateStepLink(request.ProjectID, gateCode),
                                        EstimatedTime = (int)(step.DayCount.Value * 8),
                                        UserID = step.Workers.First().EmployeeID,
                                        STT = _projectItemRepo.GetMaxSTT(request.ProjectID),
                                        IsDeleted = false,
                                        EmployeeIDRequest = _currentUser.EmployeeID,
                                        ParentID = parentProjectTaskId
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

                            // Đồng bộ checklist detail links
                            await SyncCheckListDetailLinksAsync(matchingLink.ID, step.ProjectGateStepID, step.ProjectTypeID, project);
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
                                DateEnd = step.DateEnd,
                                IsRepeat = step.IsRepeat,
                                IsDeleted = false,
                                ProjectGateStepTemplateID = step.ProjectGateStepTemplateID,
                                DepartmentID = step.DepartmentID,
                                ParentID = realParentID,
                                Content = step.Content,
                                ActualContent = step.ActualContent
                            };

                            await _stepLinkRepo.CreateAsync(newLink);

                            if (newLink.ID > 0)
                            {
                                processedLinkIDs.Add(newLink.ID);
                                idMapping[step.ID] = newLink.ID;
                                if (!existingLinks.Any(l => l.ID == newLink.ID))
                                {
                                    existingLinks.Add(newLink);
                                }

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
                                    string taskDescription = !string.IsNullOrEmpty(step.ActualContent) ? step.ActualContent : step.Content;
                                    var planStart = step.StartDate.Value;
                                    var planEnd = step.DateEnd.HasValue
                                        ? step.DateEnd.Value
                                        : (step.DayCount.Value > 0 ? planStart.AddDays((double)step.DayCount.Value - 1) : planStart);

                                    var newItem = new ProjectItem
                                    {
                                        ProjectID = request.ProjectID,
                                        TypeProjectItem = step.ProjectTypeID,
                                        Mission = mission,
                                        Description = taskDescription,
                                        PlanStartDate = planStart,
                                        PlanEndDate = planEnd,
                                        Status = 0,
                                        ItemLate = 0,
                                        NeedApprove = true,
                                        Priority = 1,
                                        Deadline = planEnd,
                                        Code = _projectItemRepo.GenerateProjectItemCodeByGateStepLink(request.ProjectID, gateCode),
                                        EstimatedTime = (int)(step.DayCount.Value * 8),
                                        UserID = step.Workers.First().EmployeeID,
                                        STT = _projectItemRepo.GetMaxSTT(request.ProjectID),
                                        IsDeleted = false,
                                        EmployeeIDRequest = _currentUser.EmployeeID,
                                        ParentID = parentProjectTaskId
                                    };
                                    _projectItemRepo.CalculateDays(newItem);
                                    if (await _projectItemRepo.CreateAsync(newItem) > 0)
                                    {
                                        newLink.ProjectTaskID = newItem.ID;
                                        await _stepLinkRepo.UpdateAsync(newLink);

                                        await SyncProjectTaskEmployees(newItem.ID, step.Workers);
                                    }
                                }

                                // Tạo Checklist detail links
                                var detailDefs = _stepCheckListDetailRepo.GetAll(d => d.ProjectGateStepID == step.ProjectGateStepID && d.IsDeleted != true).ToList();

                                // Tính PathFolder gốc cho step này (dùng chung cho tất cả rules)
                                string rootPathFolder = "";
                                if (project != null && project.CreatedDate.HasValue)
                                {
                                    int year = project.CreatedDate.Value.Year;
                                    var rootFolder = _treeFolderRepo.GetAll(f => f.ProjectTypeID == step.ProjectTypeID
                                                                              && (f.ParentID == 0 || f.ParentID == null)
                                                                              && (f.IsDeleted == null || f.IsDeleted == false)).FirstOrDefault();
                                    string typeFolderName = rootFolder?.FolderName ?? "TaiLieuChung";
                                    rootPathFolder = Path.Combine(@"D:\ProjectGate", "projects", year.ToString(), project.ProjectCode, typeFolderName);

                                    try
                                    {
                                        if (!Directory.Exists(rootPathFolder))
                                            Directory.CreateDirectory(rootPathFolder);
                                    }
                                    catch { }
                                }

                                // Lưu PathFolder vào ProjectGateStepCheckListLink để GetCheckLists có thể lấy ra
                                var existingCheckListLink = _stepCheckListLinkRepo.GetAll(cl => cl.ProjectGateStepLinkID == newLink.ID).FirstOrDefault();
                                if (existingCheckListLink == null)
                                {
                                    await _stepCheckListLinkRepo.CreateAsync(new ProjectGateStepCheckListLink
                                    {
                                        ProjectGateStepLinkID = newLink.ID,
                                        PathFolder = rootPathFolder
                                    });
                                }
                                else if (string.IsNullOrEmpty(existingCheckListLink.PathFolder) && !string.IsNullOrEmpty(rootPathFolder))
                                {
                                    existingCheckListLink.PathFolder = rootPathFolder;
                                    await _stepCheckListLinkRepo.UpdateAsync(existingCheckListLink);
                                }

                                if (detailDefs.Any())
                                {
                                    var newCheckListDetails = new List<ProjectGateStepCheckListDetailLink>();
                                    foreach (var def in detailDefs)
                                    {
                                        newCheckListDetails.Add(new ProjectGateStepCheckListDetailLink
                                        {
                                            ProjectGateStepLinkID = newLink.ID,
                                            ProjectGateStepCheckListDetailID = def.ID,
                                            IsCompleted = false,
                                            IsApprovedTBP = 0,
                                            CreatedDate = DateTime.Now,
                                            CreatedBy = _currentUser.LoginName ?? "system"
                                        });
                                    }

                                    if (newCheckListDetails.Any())
                                    {
                                        await _stepCheckListDetailLinkRepo.CreateRangeAsync(newCheckListDetails);
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
            // Chỉ lấy các nhân viên có Type == 1 (Người nhận việc) để đồng bộ với workers
            var existingWorkers = _taskEmployeeRepo.GetAll(x => x.ProjectTaskID == projectTaskId && x.Type == 1 && x.IsDeleted != true).ToList();

            // Xóa các nhân viên nhận việc (Type == 1) không còn nằm trong danh sách workers
            foreach (var ext in existingWorkers)
            {
                if (workers == null || !workers.Any(w => w.EmployeeID == ext.EmployeeID))
                {
                    ext.IsDeleted = true;
                    await _taskEmployeeRepo.UpdateAsync(ext);
                }
            }

            // Thêm nhân viên nhận việc mới hoặc kích hoạt lại các nhân viên đã bị xóa mềm
            if (workers != null)
            {
                foreach (var w in workers)
                {
                    var ext = existingWorkers.FirstOrDefault(x => x.EmployeeID == w.EmployeeID);
                    if (ext == null)
                    {
                        var deletedEmp = _taskEmployeeRepo.GetAll(x => x.ProjectTaskID == projectTaskId && x.EmployeeID == w.EmployeeID && x.Type == 1 && x.IsDeleted == true).FirstOrDefault();
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

            // Tự động gán Leader (Người liên quan - Type 2) nếu công việc chưa có người liên quan
            var hasRelated = _taskEmployeeRepo.GetAll(x => x.ProjectTaskID == projectTaskId && x.Type == 2 && x.IsDeleted != true).Any();
            if (!hasRelated && _currentUser != null && _currentUser.ID > 0)
            {
                var param2 = new { p_UserID = _currentUser.ID };
                var listLeader = await SqlDapper<UserTeam>.ProcedureToListTAsync("spGetAllLeaderTeam", param2);
                if (listLeader != null && listLeader.Any())
                {
                    foreach (var leader in listLeader)
                    {
                        if (leader.LeaderID.HasValue && leader.LeaderID.Value > 0)
                        {
                            var existingLeaderRelate = _taskEmployeeRepo.GetAll(x => x.ProjectTaskID == projectTaskId && x.EmployeeID == leader.LeaderID.Value && x.Type == 2).FirstOrDefault();
                            if (existingLeaderRelate != null)
                            {
                                if (existingLeaderRelate.IsDeleted == true)
                                {
                                    existingLeaderRelate.IsDeleted = false;
                                    await _taskEmployeeRepo.UpdateAsync(existingLeaderRelate);
                                }
                            }
                            else
                            {
                                var newEmployeeRelate = new ProjectTaskEmployee
                                {
                                    ProjectTaskID = projectTaskId,
                                    EmployeeID = leader.LeaderID.Value,
                                    Type = 2, // 2: người liên quan
                                    CanDelete = true,
                                    IsDeleted = false
                                };
                                await _taskEmployeeRepo.CreateAsync(newEmployeeRelate);
                            }
                        }
                    }
                }
            }
        }
        [RequiresPermission("N109")]
        [HttpGet("GetByProject/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            try
            {
                // SP trả về 4 result sets: links, workers, checklists+files, forms
                var (links, workers, checklistFileRows, formRows) = await SqlDapper<ProjectGateStepLinkResultDto>
                    .QueryMultipleAsync<ProjectGateStepLinkResultDto, ProjectGateStepWorkerResultDto, ProjectGateStepCheckListFileResultDto, ProjectGateStepFormResultDto>(
                        "spGetProjectGateStepLinkByProject",
                        new { ProjectID = projectId }
                    );

                var workerGroups = workers
                    .GroupBy(w => w.ProjectGateStepLinkID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var formGroups = formRows
                    .GroupBy(f => f.ProjectGateStepLinkID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Gom checklist theo LinkID, sau đó gom file theo ProjectGateStepCheckListID
                var checklistByLink = checklistFileRows
                    .GroupBy(r => r.ProjectGateStepLinkID)
                    .ToDictionary(g => g.Key, g =>
                        g.GroupBy(r => r.ProjectGateStepCheckListID)
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
                                 IsFile = first.IsFile,
                                 STT = first.STT,
                                 FileName = first.StandardFileName,
                                 IsApprovedTBP = first.IsApprovedTBP,
                                 ApprovedTBPBy = first.ApprovedTBPBy,
                                 ApprovedTBPDate = first.ApprovedTBPDate,
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
                                         Status = r.Status ?? 1
                                     }).ToList()
                             };
                         }).ToList()
                    );

                var result = links.Select(l => new ProjectGateStepLinkDto
                {
                    ID = l.ID,
                    ProjectGateStepID = l.ProjectGateStepID,
                    ProjectTypeID = l.ProjectTypeID,
                    StartDate = l.StartDate,
                    DateEnd = l.DateEnd,
                    IsRepeat = l.IsRepeat,
                    IsApproved = l.IsApproved,
                    ApprovedBy = l.ApprovedBy,
                    ApprovedDate = l.ApprovedDate,
                    ApprovalComment = l.ApprovalComment,
                    ProjectGateStepTemplateID = l.ProjectGateStepTemplateID,
                    DepartmentID = l.DepartmentID,
                    ProjectTaskID = l.ProjectTaskID,
                    ParentID = l.ParentID,
                    Content = l.Content,
                    ActualContent = l.ActualContent,
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
                        : new List<ProjectGateStepCheckListLinkDto>(),
                    Forms = formGroups.TryGetValue(l.ID, out var fList)
                        ? fList.Select(f => new ProjectGateStepFormDto
                        {
                            ID = f.ID,
                            ProjectGateStepID = f.ProjectGateStepID,
                            STT = f.STT,
                            FormName = f.FormName,
                            FileName = f.FileName,
                            FilePath = f.FilePath,
                            FileSize = f.FileSize,
                            Description = f.Description
                        }).ToList()
                        : new List<ProjectGateStepFormDto>()
                }).ToList();

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-workers-by-step-link/{stepLinkId}")]
        public async Task<IActionResult> GetWorkersByStepLink(int stepLinkId)
        {
            try
            {
                var workers = _stepWorkerRepo.GetAll(w => w.ProjectGateStepLinkID == stepLinkId).ToList();
                var employeeIds = workers.Select(w => w.EmployeeID).Distinct().ToList();
                return Ok(ApiResponseFactory.Success(employeeIds, "Lấy danh sách ID nhân viên thực hiện thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [RequiresPermission("N109")]
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

                var allRuleLinks = _stepCheckListDetailLinkRepo.GetAll(c => allLinkIds.Contains(c.ProjectGateStepLinkID) && c.IsDeleted != true).ToList();
                var allRuleLinkIds = allRuleLinks.Select(c => c.ID).ToList();

                var allRuleIds = allRuleLinks.Select(c => c.ProjectGateStepCheckListDetailID).Distinct().ToList();
                var allMasterRules = _stepCheckListDetailRepo.GetAll(r => allRuleIds.Contains(r.ID)).ToDictionary(r => r.ID);

                var allFiles = _stepFileRepo.GetAll(f => allRuleLinkIds.Contains(f.ProjectGateStepCheckListDetailLinkID) && (f.IsDeleted == false || f.IsDeleted == null)).ToList();
                var filesByRuleLink = allFiles.GroupBy(f => f.ProjectGateStepCheckListDetailLinkID).ToDictionary(g => g.Key, g => g.ToList());

                var ruleLinkGroups = allRuleLinks.GroupBy(c => c.ProjectGateStepLinkID).ToDictionary(g => g.Key, g => g.ToList());

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
                    ProjectGateStepTemplateID = l.ProjectGateStepTemplateID,
                    DepartmentID = l.DepartmentID,
                    ProjectTaskID = l.ProjectTaskID,
                    Workers = workerGroups.TryGetValue(l.ID, out var wList)
                        ? wList.Select(w => new ProjectGateStepWorkerDto
                        {
                            EmployeeID = w.EmployeeID,
                            DayCount = w.DayCount,
                            UnitPrice = w.UnitPrice,
                            TotalAmount = w.TotalAmount
                        }).ToList()
                        : new List<ProjectGateStepWorkerDto>(),
                    CheckLists = ruleLinkGroups.TryGetValue(l.ID, out var rList)
                        ? rList.Select(c =>
                        {
                            var master = allMasterRules.TryGetValue(c.ProjectGateStepCheckListDetailID, out var m) ? m : null;
                            return new ProjectGateStepCheckListLinkDto
                            {
                                ID = c.ID,
                                ProjectGateStepCheckListID = c.ProjectGateStepCheckListDetailID,
                                IsPass = c.IsCompleted,
                                IsRequired = master?.IsCheck ?? false,
                                Description = master?.FileRule,
                                Type = master?.FileFormat,
                                IsFile = master?.IsFile ?? true,
                                STT = master?.STT,
                                FileName = master?.FileName,
                                IsApprovedTBP = c.IsApprovedTBP,
                                ApprovedTBPBy = c.ApprovedTBPBy,
                                ApprovedTBPDate = c.ApprovedTBPDate,
                                Files = filesByRuleLink.TryGetValue(c.ID, out var fList)
                                    ? fList.Select(f => new ProjectGateStepFileDto
                                    {
                                        ID = f.ID,
                                        FileName = f.FileName,
                                        FilePath = f.FilePath,
                                        FileSize = f.FileSize,
                                        ContentType = f.ContentType,
                                        CreatedBy = f.CreatedBy,
                                        CreatedDate = f.CreatedDate,
                                        Status = f.Status ?? 1
                                    }).ToList()
                                    : new List<ProjectGateStepFileDto>()
                            };
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


        [HttpPost("Reject/{linkId}")]
        public async Task<IActionResult> Reject(int linkId, [FromBody] string? comment)
        {
            try
            {
                var link = _stepLinkRepo.GetByID(linkId);
                if (link == null)
                    return NotFound(ApiResponseFactory.Fail(null, "Không tìm thấy bước công việc"));

                await _stepLinkRepo.UpdateAsync(link);
                return Ok(ApiResponseFactory.Success(true, "Đã từ chối phê duyệt công đoạn"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }



        /// <summary>
        /// Duyệt hoặc hủy duyệt nhiều công đoạn cùng lúc.
        /// Nếu IsApproved=true và ForceApprove=false: kiểm tra checklist TBP chưa duyệt trước,
        /// nếu có thì trả về HasPendingTBP=true để frontend hỏi người dùng.
        /// </summary>

        [HttpPost("ApproveMultiple")]
        [RequiresPermission("N110")]
        public async Task<IActionResult> ApproveMultiple([FromBody] ApproveMultipleDto request)
        {
            try
            {
                if (request == null || request.LinkIDs == null || !request.LinkIDs.Any())
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách công đoạn không được rỗng"));

                // Khi duyệt (IsApproved=true) và chưa force: kiểm tra TBP pending
                if (request.IsApproved && !request.ForceApprove)
                {
                    bool hasPending = await _stepLinkRepo.CheckPendingTbpAsync(request.LinkIDs);
                    if (hasPending)
                    {
                        return Ok(ApiResponseFactory.Success(new ApproveMultipleResultDto
                        {
                            Success = false,
                            HasPendingTBP = true,
                            Message = "Có công đoạn chứa checklist chưa được Trưởng bộ phận duyệt. Bạn có muốn tiếp tục duyệt không?"
                        }, "Cần xác nhận"));
                    }
                }

                // Tiến hành duyệt / hủy duyệt
                var updatedCount = 0;
                foreach (var linkId in request.LinkIDs)
                {
                    var link = _stepLinkRepo.GetByID(linkId);
                    if (link == null) continue;

                    link.IsApproved = request.IsApproved;
                    link.ApprovedBy = request.IsApproved ? _currentUser.LoginName : null;
                    link.ApprovedDate = request.IsApproved ? DateTime.Now : null;

                    await _stepLinkRepo.UpdateAsync(link);
                    updatedCount++;
                }

                var action = request.IsApproved ? "Duyệt" : "Hủy duyệt";
                return Ok(ApiResponseFactory.Success(new ApproveMultipleResultDto
                {
                    Success = true,
                    HasPendingTBP = false,
                    Message = $"{action} thành công {updatedCount} công đoạn"
                }, $"{action} thành công {updatedCount} công đoạn"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        private async Task SyncCheckListDetailLinksAsync(int stepLinkId, int gateStepId, int projectTypeId, RERPAPI.Model.Entities.Project project)
        {
            var detailDefs = _stepCheckListDetailRepo.GetAll(d => d.ProjectGateStepID == gateStepId && d.IsDeleted != true).ToList();
            if (!detailDefs.Any()) return;

            var existingLinks = _stepCheckListDetailLinkRepo.GetAll(cl => cl.ProjectGateStepLinkID == stepLinkId && cl.IsDeleted != true).ToList();

            // Tính PathFolder gốc và đảm bảo ProjectGateStepCheckListLink có PathFolder
            if (project != null && project.CreatedDate.HasValue)
            {
                int year = project.CreatedDate.Value.Year;
                var rootFolder = _treeFolderRepo.GetAll(f => f.ProjectTypeID == projectTypeId
                                                          && (f.ParentID == 0 || f.ParentID == null)
                                                          && (f.IsDeleted == null || f.IsDeleted == false)).FirstOrDefault();
                string typeFolderName = rootFolder?.FolderName ?? "TaiLieuChung";
                string rootPathFolder = Path.Combine(@"D:\ProjectGate", "projects", year.ToString(), project.ProjectCode, typeFolderName);

                try { if (!Directory.Exists(rootPathFolder)) Directory.CreateDirectory(rootPathFolder); } catch { }

                var existingCheckListLink = _stepCheckListLinkRepo.GetAll(cl => cl.ProjectGateStepLinkID == stepLinkId).FirstOrDefault();
                if (existingCheckListLink == null)
                {
                    await _stepCheckListLinkRepo.CreateAsync(new ProjectGateStepCheckListLink
                    {
                        ProjectGateStepLinkID = stepLinkId,
                        PathFolder = rootPathFolder
                    });
                }
                else if (string.IsNullOrEmpty(existingCheckListLink.PathFolder) && !string.IsNullOrEmpty(rootPathFolder))
                {
                    existingCheckListLink.PathFolder = rootPathFolder;
                    await _stepCheckListLinkRepo.UpdateAsync(existingCheckListLink);
                }
            }

            var newDetails = new List<ProjectGateStepCheckListDetailLink>();
            foreach (var def in detailDefs)
            {
                if (!existingLinks.Any(cl => cl.ProjectGateStepCheckListDetailID == def.ID))
                {
                    newDetails.Add(new ProjectGateStepCheckListDetailLink
                    {
                        ProjectGateStepLinkID = stepLinkId,
                        ProjectGateStepCheckListDetailID = def.ID,
                        IsCompleted = false,
                        IsApprovedTBP = 0,
                        CreatedDate = DateTime.Now,
                        CreatedBy = _currentUser.LoginName ?? "system"
                    });
                }
            }

            if (newDetails.Any())
            {
                await _stepCheckListDetailLinkRepo.CreateRangeAsync(newDetails);
            }
        }

        [HttpGet("GetProjectItemParentChild/{projectTaskId}")]
        public async Task<IActionResult> GetProjectItemParentChild(int projectTaskId)
        {
            try
            {
                var param = new { ProjectTaskID = projectTaskId };
                var list = await SqlDapper<object>.ProcedureToListTAsync("spGetProjectItemParentChild", param);
                return Ok(ApiResponseFactory.Success(list));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
