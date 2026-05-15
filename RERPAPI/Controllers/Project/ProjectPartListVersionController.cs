using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;
using System.Linq.Expressions;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectPartListVersionController : ControllerBase
    {
        private ProjectPartlistVersionRepo _projectPartlistVersionRepo;
        private ProjectPartListRepo _projectPartListRepo;
        private readonly ProjectHistoryProblemRepo _projectHistoryProblemRepo;
        private readonly ProjectHistoryProblemPartListLinkRepo _projectHistoryProblemPartListLinkRepo;

        public ProjectPartListVersionController(ProjectPartlistVersionRepo projectPartlistVersionRepo, ProjectPartListRepo projectPartListRepo, ProjectHistoryProblemRepo projectHistoryProblemRepo, ProjectHistoryProblemPartListLinkRepo projectHistoryProblemPartListLinkRepo)
        {
            _projectPartlistVersionRepo = projectPartlistVersionRepo;
            _projectPartListRepo = projectPartListRepo;
            _projectHistoryProblemRepo = projectHistoryProblemRepo;
            _projectHistoryProblemPartListLinkRepo = projectHistoryProblemPartListLinkRepo;

        }
        [HttpGet("get-all")]
        public IActionResult GetAll(int projectSolutionId, bool isPO)
        {
            try
            {
                int statusVersion = 1;
                if (isPO)
                {
                    statusVersion = 2;
                }
                var projectPartListVersions = SQLHelper<dynamic>.ProcedureToList(
                    "spGetProjectPartListVersion",
                    new string[] { "@ProjectSolutionID", "@StatusVersion" },
                    new object[] { projectSolutionId, statusVersion });
                return Ok(ApiResponseFactory.Success(
                    SQLHelper<object>.GetListData(projectPartListVersions, 0),
                    ""
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] SaveProjectPartListVersionRequest request)
        {
            try
            {
                var version = request.ProjectPartListVersion;
                string message = "";
                int ID = 0;
                if (!_projectPartlistVersionRepo.Validate(version, out message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (version.ID > 0)
                {
                    await _projectPartlistVersionRepo.UpdateAsync(version);
                    ID = version.ID;
                }
                else
                {
                    await _projectPartlistVersionRepo.CreateAsync(version);
                    ID = version.ID;
                }
                if (version.IsActive == false)
                {
                    var myDict = new Dictionary<Expression<Func<ProjectPartList, object>>, object>
                            {
                                { x => x.IsApprovedTBP, false },
                                { x => x.IsApprovedPurchase, false }

                            };
                    await _projectPartListRepo.UpdateFieldByAttributeAsync(x => x.ProjectPartListVersionID == ID, // Bây giờ mới có ID đúng
        myDict);
                }
                if (version.IsDeleted == true)
                {
                    var myDict = new Dictionary<Expression<Func<ProjectPartList, object>>, object>
                            {
                                { x => x.IsDeleted, true },
                                { x => x.ReasonDeleted, version.ReasonDeleted}
                            };
                    await _projectPartListRepo.UpdateFieldByAttributeAsync(x => x.ProjectPartListVersionID == version.ID, myDict);
                }

                // Lưu bảng link ProjectHistoryProblemPartListLink
                var inputProblemIds = (request.ProjectHistoryProblemIDs ?? new List<int>())
                    .Distinct()
                    .ToList();

                var validProblemIds = _projectHistoryProblemRepo
                    .GetAll(x => inputProblemIds.Contains(x.ID) && x.IsDeleted == false)
                    .Select(x => x.ID)
                    .Distinct()
                    .ToList();

                var oldLinks = _projectHistoryProblemPartListLinkRepo
                    .GetAll(x => x.ProjectPartListVersionID == ID && x.IsDeleted == false);

                if (oldLinks != null && oldLinks.Count > 0)
                {
                    foreach (var oldLink in oldLinks)
                    {
                        oldLink.IsDeleted = true;
                        await _projectHistoryProblemPartListLinkRepo.UpdateAsync(oldLink);
                    }
                }

                foreach (var problemId in validProblemIds)
                {
                    var newLink = new ProjectHistoryProblemPartListLink
                    {
                        ProjectHistoryProblemID = problemId,
                        ProjectPartListVersionID = ID,
                        IsDeleted = false
                    };

                    await _projectHistoryProblemPartListLinkRepo.CreateAsync(newLink);
                }
                return Ok(ApiResponseFactory.Success(request, "Cập nhật dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi lưu phiên bản danh sách phần"));
            }
        }

        [HttpGet("get-cbb-version")]
        public IActionResult GetCBBVersion(int projectSolutionId)
        {
            try
            {
                var projectPartListVersions = SQLHelper<dynamic>.ProcedureToList(
                    "spGetProjectPartListVersion",
                    new string[] { "@ProjectSolutionID" },
                    new object[] { projectSolutionId });
                return Ok(ApiResponseFactory.Success(
                    SQLHelper<object>.GetListData(projectPartListVersions, 0),
                    ""
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        // lấy danh sách phát sinh theo dự án

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

        //lấy phát sinh đã link với version đang chọn
        [HttpGet("get-project-history-problem-linked")]
        public IActionResult GetProjectHistoryProblemLinked(int projectPartListVersionID)
        {
            try
            {
                var problemIds = _projectHistoryProblemPartListLinkRepo
                    .GetAll(x => x.ProjectPartListVersionID == projectPartListVersionID && x.IsDeleted == false)
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
    public class SaveProjectPartListVersionRequest
    {
        public ProjectPartListVersion ProjectPartListVersion { get; set; }
        public List<int>? ProjectHistoryProblemIDs { get; set; }
    }

}