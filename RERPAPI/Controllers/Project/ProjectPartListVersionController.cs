using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Duan.MeetingMinutes;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using RERPAPI.Middleware;
using RERPAPI.Model.DTO;

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
        private ProjectSolutionRepo _projectSolutionRepo;
        private readonly ProjectHistoryProblemPartListLinkRepo _projectHistoryProblemPartListLinkRepo;
        private readonly ProjectPartListHistoryLogRepo _partListHistoryLogRepo;
        private ProjectRequestRepo _projectRequestRepo;
        private ProjectRepo _projectRepo;
        private readonly IConfiguration _configuration;

        public ProjectPartListVersionController(
            ProjectPartlistVersionRepo projectPartlistVersionRepo,
            ProjectPartListRepo projectPartListRepo,
            ProjectHistoryProblemRepo projectHistoryProblemRepo,
            ProjectHistoryProblemPartListLinkRepo projectHistoryProblemPartListLinkRepo,
            ProjectPartListHistoryLogRepo partListHistoryLogRepo,
            ProjectSolutionRepo projectSolutionRepo,
            ProjectRepo projectRepo,
            ProjectRequestRepo projectRequestRepo,
            IConfiguration configuration)
        {
            _projectPartlistVersionRepo = projectPartlistVersionRepo;
            _projectPartListRepo = projectPartListRepo;
            _projectHistoryProblemRepo = projectHistoryProblemRepo;
            _projectHistoryProblemPartListLinkRepo = projectHistoryProblemPartListLinkRepo;
            _partListHistoryLogRepo = partListHistoryLogRepo;
            _projectSolutionRepo = projectSolutionRepo;
            _projectRepo = projectRepo;
            _projectRequestRepo = projectRequestRepo;
            _configuration = configuration;
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
                int? projectId = version.ProjectID;
                string versionCode = version.Code;
                string descriptionVersion = version.DescriptionVersion;
                int? statusVersion = version.StatusVersion;

                string typeName = statusVersion == 2 ? "phiên bản PO" : "phiên bản GP";
                string actionText = version.ID > 0 ? $"Cập nhật {typeName}" : $"Thêm mới {typeName}";

                ProjectPartListVersion oldClone = null;
                var projectSolution = _projectSolutionRepo.GetByID(version.ProjectSolutionID ?? 0);
                var projectRequest = _projectRequestRepo.GetByID(projectSolution.ProjectRequestID ?? 0);
                int projectID = projectRequest.ProjectID ?? 0;
                if (version.ID > 0)
                {
                    var oldVersion = await _projectPartlistVersionRepo.GetByIDAsync(version.ID);
                    if (oldVersion != null)
                    {
                        oldClone = new ProjectPartListVersion
                        {
                            Code = oldVersion.Code,
                            DescriptionVersion = oldVersion.DescriptionVersion,
                            StatusVersion = oldVersion.StatusVersion,
                            IsActive = oldVersion.IsActive,
                            IsApproved = oldVersion.IsApproved,
                            IsDeleted = oldVersion.IsDeleted,
                            IsConsumable = oldVersion.IsConsumable,
                            ProjectTypeID = oldVersion.ProjectTypeID
                        };

                        if (projectId == null || projectId <= 0)
                        {
                            projectId = oldVersion.ProjectID;
                        }
                        if (version.ProjectSolutionID == null || version.ProjectSolutionID <= 0)
                        {
                            version.ProjectSolutionID = oldVersion.ProjectSolutionID;
                        }
                        if (string.IsNullOrEmpty(versionCode))
                        {
                            versionCode = oldVersion.Code;
                        }
                        if (string.IsNullOrEmpty(descriptionVersion))
                        {
                            descriptionVersion = oldVersion.DescriptionVersion;
                        }
                        if (statusVersion == null || statusVersion <= 0)
                        {
                            statusVersion = oldVersion.StatusVersion;
                            version.StatusVersion = oldVersion.StatusVersion;
                        }

                        bool isCodeChanged = version.Code != null && version.Code != oldVersion.Code;
                        bool isDescChanged = version.DescriptionVersion != null && version.DescriptionVersion != oldVersion.DescriptionVersion;
                        bool isActiveChanged = version.IsActive != null && version.IsActive != oldVersion.IsActive;
                        bool isApprovedChanged = version.IsApproved != null && version.IsApproved != oldVersion.IsApproved;
                        bool isStatusChanged = version.StatusVersion != null && version.StatusVersion != oldVersion.StatusVersion;

                        int changeCount = (isCodeChanged ? 1 : 0) +
                                          (isDescChanged ? 1 : 0) +
                                          (isActiveChanged ? 1 : 0) +
                                          (isApprovedChanged ? 1 : 0) +
                                          (isStatusChanged ? 1 : 0);

                        if (changeCount > 1)
                        {
                            actionText = $"Cập nhật {typeName}";
                        }
                        else
                        {
                            if (isActiveChanged)
                            {
                                actionText = version.IsActive == true ? $"Sử dụng {typeName}" : $"Bỏ sử dụng {typeName}";
                            }
                            else if (isApprovedChanged)
                            {
                                actionText = version.IsApproved == true ? $"Duyệt {typeName}" : $"Hủy duyệt {typeName}";
                            }
                            else
                            {
                                actionText = $"Cập nhật {typeName}";
                            }
                        }
                    }

                    await _projectPartlistVersionRepo.UpdateAsync(version);
                    ID = version.ID;
                }
                else
                {
                    await _projectPartlistVersionRepo.CreateAsync(version);
                    ID = version.ID;
                }

                if (version.IsDeleted == true) actionText = $"Xóa {typeName}";

                if ((projectId == null || projectId <= 0) && version.ProjectSolutionID.HasValue && version.ProjectSolutionID.Value > 0)
                {
                    projectId = await _projectPartlistVersionRepo.GetProjectIdFromSolutionAsync(version.ProjectSolutionID.Value);
                }

                // Log version history
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);

                string contentLog = $"[{currentUser.FullName}] đã {actionText.ToLower()} [{versionCode}]";
                bool shouldLog = true;
                if (version.ID > 0 && oldClone != null)
                {
                    var diff = _partListHistoryLogRepo.BuildVersionDiff(oldClone, version);
                    if (string.IsNullOrEmpty(diff))
                    {
                        shouldLog = false;
                    }
                    else
                    {
                        if (actionText.StartsWith("Cập nhật"))
                        {
                            contentLog = $"[{currentUser.FullName}] đã cập nhật phiên bản {(statusVersion == 2 ? "po" : "gp")}:\n{diff}";
                        }
                        else
                        {
                            contentLog += $".\nChi tiết thay đổi:\n{diff}";
                        }
                    }
                }
                else
                {
                    contentLog += $" - {descriptionVersion}";
                }

                if (shouldLog)
                {
                    await _partListHistoryLogRepo.AddLog(
                        projectID,
                        ID,
                        null,
                        actionText,
                        contentLog,
                        currentUser.LoginName,
                        currentUser.EmployeeID);
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