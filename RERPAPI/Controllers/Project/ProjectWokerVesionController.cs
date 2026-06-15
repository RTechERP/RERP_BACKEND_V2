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

        private readonly ProjectHistoryProblemWorkerLinkRepo _projectHistoryProblemWorkerLinkRepo;
        private readonly ProjectHistoryProblemRepo _projectHistoryProblemRepo;

        public ProjectWokerVesionController(
          ProjectWorkerVersionRepo projectWorkerVersionRepo,
          ProjectHistoryProblemWorkerLinkRepo projectHistoryProblemWorkerLinkRepo,
          ProjectHistoryProblemRepo projectHistoryProblemRepo
      )
        {
            _projectWorkerVersionRepo = projectWorkerVersionRepo;
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