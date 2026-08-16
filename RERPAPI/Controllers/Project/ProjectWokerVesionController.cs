using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectWokerVesionController : ControllerBase
    {
        //nhân công dự án
        private readonly ProjectWorkerVersionRepo _projectWorkerVersionRepo;
        private readonly ProjectWorkerRepo _projectWorkerRepo;

        private readonly ProjectHistoryProblemWorkerLinkRepo _projectHistoryProblemWorkerLinkRepo;
        private readonly ProjectHistoryProblemRepo _projectHistoryProblemRepo;

        public ProjectWokerVesionController(
          ProjectWorkerVersionRepo projectWorkerVersionRepo,
          ProjectWorkerRepo projectWorkerRepo,
          ProjectHistoryProblemWorkerLinkRepo projectHistoryProblemWorkerLinkRepo,
          ProjectHistoryProblemRepo projectHistoryProblemRepo
      )
        {
            _projectWorkerVersionRepo = projectWorkerVersionRepo;
            _projectWorkerRepo = projectWorkerRepo;
            _projectHistoryProblemWorkerLinkRepo = projectHistoryProblemWorkerLinkRepo;
            _projectHistoryProblemRepo = projectHistoryProblemRepo;
        }

        [HttpPost("save-worker-version")]
        public async Task<IActionResult> SaveWorkerVersion([FromBody] SaveProjectWorkerVersionDTO request)
        {
            try
            {
                var item = request.ProjectWorkerVersion;
                string message = "";
                if (!_projectWorkerVersionRepo.Validate(item, out message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (item.ID > 0)
                {
                    await _projectWorkerVersionRepo.UpdateAsync(item);
                }
                else
                {
                    if (item.StatusVersion == 2)
                    {
                        var check = _projectWorkerVersionRepo.GetAll(x => x.ProjectSolutionID == item.ProjectSolutionID && x.StatusVersion == 2 && x.IsDeleted == false && x.ProjectTypeID == item.ProjectTypeID);
                        if (check.Count > 0)
                        {
                            return Ok(new { status = 2, message = $"Danh mục vừa chọn đã có phiên bản Po" });
                        }
                    }

                    await _projectWorkerVersionRepo.CreateAsync(item);
                }

                // Sau khi save xong phải lấy được ID của ProjectWorkerVersion
                var workerVersionId = item.ID;

                // Xử lý bảng link n-n ProjectHistoryProblemWorkerLink
                if (workerVersionId > 0)
                {
                    // 1. Xóa mềm link cũ
                    var oldLinks = _projectHistoryProblemWorkerLinkRepo
                        .GetAll(x => x.ProjectWorkerVersionID == workerVersionId && x.IsDeleted == false);

                    if (oldLinks != null && oldLinks.Count > 0)
                    {
                        foreach (var oldLink in oldLinks)
                        {
                            oldLink.IsDeleted = true;
                            await _projectHistoryProblemWorkerLinkRepo.UpdateAsync(oldLink);
                        }
                    }

                    // 2. Insert lại link mới
                    if (request.ProjectHistoryProblemIds != null && request.ProjectHistoryProblemIds.Count > 0)
                    {
                        var validProblemIds = _projectHistoryProblemRepo
                            .GetAll(x => request.ProjectHistoryProblemIds.Contains(x.ID) && x.IsDeleted == false)
                            .Select(x => x.ID)
                            .Distinct()
                            .ToList();

                        foreach (var problemId in validProblemIds)
                        {
                            var newLink = new ProjectHistoryProblemWorkerLink
                            {
                                ProjectHistoryProblemID = problemId,
                                ProjectWorkerVersionID = workerVersionId,
                                IsDeleted = false
                            };

                            await _projectHistoryProblemWorkerLinkRepo.CreateAsync(newLink);
                        }
                    }
                }

                return Ok(ApiResponseFactory.Success(item, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Chuyển phiên bản giải pháp (StatusVersion = 1) thành phiên bản PO (StatusVersion = 2).
        /// Toàn bộ nhân công của phiên bản giải pháp được tạo mới y hệt sang phiên bản PO.
        /// </summary>
        [HttpPost("convert-versionPO")]
        public async Task<IActionResult> ConvertVersionPO([FromBody] ConvertWorkerVersionPODTO request)
        {
            try
            {
                ProjectWorkerVersion versionModel = _projectWorkerVersionRepo.GetByID(request.ID);
                if (versionModel == null || versionModel.IsDeleted == true)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy phiên bản giải pháp!"));
                }

                if (versionModel.StatusVersion != 1)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng chọn phiên bản giải pháp để chuyển thành PO!"));
                }

                // Quy ước của spGetProjectWorkerVersion_New: ISNULL(IsDeleted, 0) <> 1, tức NULL = chưa xóa
                var versions = _projectWorkerVersionRepo.GetAll(
                    x => x.ProjectTypeID == versionModel.ProjectTypeID &&
                         x.StatusVersion == 2 &&
                         x.ProjectSolutionID == versionModel.ProjectSolutionID &&
                         x.IsDeleted != true
                    );

                if (versions.Count > 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Danh mục [{request.ProjectTypeName}] đã có phiên bản PO!"));
                }

                ProjectWorkerVersion newVersion = new ProjectWorkerVersion();
                newVersion.ProjectID = request.ProjectID ?? versionModel.ProjectID;
                newVersion.STT = versionModel.STT;
                newVersion.Code = versionModel.Code;
                newVersion.DescriptionVersion = versionModel.DescriptionVersion;
                newVersion.ProjectSolutionID = versionModel.ProjectSolutionID;
                newVersion.ProjectTypeID = versionModel.ProjectTypeID;
                newVersion.StatusVersion = 2;
                newVersion.IsActive = false;
                newVersion.IsApprovedTBP = false;
                newVersion.IsDeleted = false;
                newVersion.IsProblem = versionModel.IsProblem;
                newVersion.ProjectHistoryProblemID = versionModel.ProjectHistoryProblemID;

                await _projectWorkerVersionRepo.CreateAsync(newVersion);

                await CopyWorkerToNewVersion(newVersion.ID, versionModel.ID, newVersion.ProjectID ?? 0);

                return Ok(ApiResponseFactory.Success(newVersion, "Đã chuyển phiên bản giải pháp thành PO!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        /// <summary>
        /// Copy toàn bộ nhân công của phiên bản cũ sang phiên bản mới, giữ nguyên cây cha - con.
        /// </summary>
        [NonAction]
        public async Task CopyWorkerToNewVersion(int newVersionID, int oldVersionID, int projectID)
        {
            // Sắp xếp theo độ sâu của TT để nút cha luôn được tạo trước nút con
            List<ProjectWorker> oldWorkers = _projectWorkerRepo
                .GetAll(x => x.ProjectWorkerVersionID == oldVersionID && x.IsDeleted != true)
                .OrderBy(x => string.IsNullOrEmpty(x.TT) ? 0 : x.TT.Split('.').Length)
                .ThenBy(x => x.ID)
                .ToList();

            // Ánh xạ ID cũ -> ID mới để gán lại ParentID
            var oldIdToNewIdMap = new Dictionary<int, int>();

            foreach (ProjectWorker item in oldWorkers)
            {
                ProjectWorker newWorker = new ProjectWorker();
                newWorker.TT = item.TT;
                newWorker.WorkContent = item.WorkContent;
                newWorker.AmountPeople = item.AmountPeople;
                newWorker.NumberOfDay = item.NumberOfDay;
                newWorker.TotalWorkforce = item.TotalWorkforce;
                newWorker.Price = item.Price;
                newWorker.TotalPrice = item.TotalPrice;
                newWorker.ProjectWorkerTypeID = item.ProjectWorkerTypeID;
                newWorker.ProjectID = projectID > 0 ? projectID : item.ProjectID;
                newWorker.ProjectTypeID = item.ProjectTypeID;
                newWorker.ProjectSolutionID = item.ProjectSolutionID;
                newWorker.ProjectWorkerVersionID = newVersionID;
                newWorker.StatusVersion = 2;
                newWorker.IsApprovedTBP = false;
                newWorker.IsDeleted = false;

                if (item.ParentID.HasValue && item.ParentID.Value > 0
                    && oldIdToNewIdMap.ContainsKey(item.ParentID.Value))
                {
                    newWorker.ParentID = oldIdToNewIdMap[item.ParentID.Value];
                }
                else
                {
                    newWorker.ParentID = 0;
                }

                await _projectWorkerRepo.CreateAsync(newWorker);

                oldIdToNewIdMap[item.ID] = newWorker.ID;
            }
        }

        [HttpGet("get-project-solution-cbb/{projectID}")]
        public async Task<IActionResult> GetProjectSolution(int projectID)
        {
            try
            {
                var result = SQLHelper<object>.ProcedureToList("spGetProjectSolution", new string[] { "@ProjectID" }, new object[] { projectID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-project-history-problem-by-project")]
        public IActionResult GetProjectHistoryProblemByProject(int projectID)
        {
            try
            {
                var data = _projectHistoryProblemRepo
                    .GetAll(x => x.ProjectID == projectID && x.IsDeleted == false)
                    .OrderByDescending(x => x.DateProblem)
                    .ThenByDescending(x => x.ID)
                    .Select(x => new
                    {
                        x.ID,
                        x.ProjectID,
                        x.DateProblem,
                        x.ContentError,
                        x.Remedies,
                        x.EmployeeID,
                        x.IsApproved_PM,
                        x.IsApproved_PP,
                        x.IsApproved_TP
                    })
                    .ToList();

                return Ok(ApiResponseFactory.Success(data, "Lấy danh sách ProjectHistoryProblem thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-project-history-problem-linked")]
        public IActionResult GetProjectHistoryProblemLinked(int projectWorkerVersionID)
        {
            try
            {
                var problemIds = _projectHistoryProblemWorkerLinkRepo
                    .GetAll(x => x.ProjectWorkerVersionID == projectWorkerVersionID && x.IsDeleted == false)
                    .Select(x => x.ProjectHistoryProblemID)
                    .Distinct()
                    .ToList();

                var data = _projectHistoryProblemRepo
                    .GetAll(x => problemIds.Contains(x.ID) && x.IsDeleted == false)
                    .OrderByDescending(x => x.DateProblem)
                    .ThenByDescending(x => x.ID)
                    .Select(x => new
                    {
                        x.ID,
                        x.ProjectID,
                        x.DateProblem,
                        x.ContentError,
                        x.Remedies,
                        x.EmployeeID,
                        x.IsApproved_PM,
                        x.IsApproved_PP,
                        x.IsApproved_TP
                    })
                    .ToList();

                return Ok(ApiResponseFactory.Success(data, "Lấy danh sách ProjectHistoryProblem đã liên kết thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}