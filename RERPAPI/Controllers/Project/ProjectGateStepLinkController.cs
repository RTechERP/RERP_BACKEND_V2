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
        private readonly ProjectRepo _projectRepo;
        private readonly ProjectTreeFolderRepo _treeFolderRepo;
        private readonly ProjectItemRepo _projectItemRepo;

        public ProjectGateStepLinkController(
            ProjectGateStepLinkRepo stepLinkRepo, 
            ProjectGateStepWorkerRepo stepWorkerRepo,
            ProjectGateStepCheckListLinkRepo stepCheckListLinkRepo,
            ProjectGateStepCheckListRepo stepCheckListRepo,
            ProjectRepo projectRepo,
            ProjectTreeFolderRepo treeFolderRepo,
            ProjectItemRepo projectItemRepo)
        {
            _stepLinkRepo = stepLinkRepo;
            _stepWorkerRepo = stepWorkerRepo;
            _stepCheckListLinkRepo = stepCheckListLinkRepo;
            _stepCheckListRepo = stepCheckListRepo;
            _projectRepo = projectRepo;
            _treeFolderRepo = treeFolderRepo;
            _projectItemRepo = projectItemRepo;
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
                                        projectItem.EstimatedTime = (int)step.DayCount.Value;
                                        projectItem.UserID = step.Workers.First().EmployeeID;
                                        projectItem.IsDeleted = false;
                                        _projectItemRepo.CalculateDays(projectItem);
                                        await _projectItemRepo.UpdateAsync(projectItem);
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
                                        Status = 0,
                                        ItemLate = 0,
                                        Code = _projectItemRepo.GenerateProjectItemCode(request.ProjectID),
                                        EstimatedTime = (int)step.DayCount.Value,
                                        UserID = step.Workers.First().EmployeeID,
                                        STT = _projectItemRepo.GetMaxSTT(request.ProjectID),
                                        IsDeleted = false
                                    };
                                    _projectItemRepo.CalculateDays(newItem);
                                    if (await _projectItemRepo.CreateAsync(newItem) > 0)
                                    {
                                        matchingLink.ProjectTaskID = newItem.ID;
                                        await _stepLinkRepo.UpdateAsync(matchingLink);
                                    }
                                }
                            }
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
                                        NeedApprove=true,
                                        Priority=1,
                                        Deadline = planEnd,
                                        Code = _projectItemRepo.GenerateProjectItemCode(request.ProjectID),
                                        EstimatedTime = (int)step.DayCount.Value,
                                        UserID = step.Workers.First().EmployeeID,
                                        STT = _projectItemRepo.GetMaxSTT(request.ProjectID),
                                        IsDeleted = false
                                    };
                                    _projectItemRepo.CalculateDays(newItem);
                                    if (await _projectItemRepo.CreateAsync(newItem) > 0)
                                    {
                                        newLink.ProjectTaskID = newItem.ID;
                                        await _stepLinkRepo.UpdateAsync(newLink);
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
                                            catch {}
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

        [HttpGet("GetByProject/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            try
            {
                var (links, workers) = await SqlDapper<ProjectGateStepLinkResultDto>
                    .QueryMultipleAsync<ProjectGateStepLinkResultDto, ProjectGateStepWorkerResultDto>(
                        "spGetProjectGateStepLinkByProject",
                        new { ProjectID = projectId }
                    );

                var workerGroups = workers
                    .GroupBy(w => w.ProjectGateStepLinkID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var allLinkIds = links.Select(x => x.ID).ToList();
                var allChecklists = _stepCheckListLinkRepo.GetAll(c => allLinkIds.Contains(c.ProjectGateStepLinkID ?? 0));
                var checklistGroups = allChecklists.GroupBy(c => c.ProjectGateStepLinkID).ToDictionary(g => g.Key, g => g.ToList());

                var result = links.Select(l => new ProjectGateStepLinkDto
                {
                    ProjectGateStepID = l.ProjectGateStepID,
                    ProjectTypeID = l.ProjectTypeID,
                    StartDate = l.StartDate,
                    IsRepeat = l.IsRepeat,
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
                            ProjectGateStepCheckListID = c.ProjectGateStepCheckListID,
                            PathFolder = c.PathFolder,
                            IsPass = c.IsPass
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
                var allChecklists = _stepCheckListLinkRepo.GetAll(c => allLinkIds.Contains(c.ProjectGateStepLinkID ?? 0));
                var checklistGroups = allChecklists.GroupBy(c => c.ProjectGateStepLinkID).ToDictionary(g => g.Key, g => g.ToList());

                var result = links.Select(l => new ProjectGateStepLinkDto
                {
                    ProjectGateStepID = l.ProjectGateStepID,
                    ProjectTypeID = l.ProjectTypeID,
                    StartDate = l.StartDate,
                    IsRepeat = l.IsRepeat,
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
                            ProjectGateStepCheckListID = c.ProjectGateStepCheckListID,
                            PathFolder = c.PathFolder,
                            IsPass = c.IsPass
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
    }
}
