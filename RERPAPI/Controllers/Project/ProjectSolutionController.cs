using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Middleware;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Repo.GenericEntity.Project;
using System.Threading.Tasks;
namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectSolutionController : ControllerBase
    {
        private ProjectSolutionRepo _projectSolutionRepo;
        private readonly IConfiguration _configuration;
        private readonly ProjectRepo _projectRepo;
        private readonly ProjectRequestRepo _projectRequestRepo;
        private readonly ProjectSolutionFileRepo _projectSolutionFilRepo;
        private readonly ProjectRequestFileRepo _projectRequestFileRepo;
        public ProjectSolutionController(
            ProjectSolutionRepo projectSolutionRepo,
            IConfiguration configuration,
            ProjectRepo projectRepo,
            ProjectRequestRepo projectRequestRepo,
            ProjectSolutionFileRepo projectSolutionFilRepo,
            ProjectRequestFileRepo projectRequestFileRepo)
        {
            _projectSolutionRepo = projectSolutionRepo;
            _configuration = configuration;
            _projectRepo = projectRepo;
            _projectRequestRepo = projectRequestRepo;
            _projectSolutionFilRepo = projectSolutionFilRepo;
            _projectRequestFileRepo = projectRequestFileRepo;
        }
        [HttpGet("get-all-project")]
        public async Task<IActionResult> GetAllProject()
        {
            try
            {
                List<Model.Entities.Project> dtProject = _projectRepo.GetAll(x => x.IsDeleted == false).OrderByDescending(x => x.ID).ToList();
                return Ok(ApiResponseFactory.Success(dtProject, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-all-project-request")]
        public async Task<IActionResult> GetAllProjectRequest(int projectID)
        {
            try
            {
                List<ProjectRequest> dtProject = _projectRequestRepo.GetAll(x => x.IsDeleted == false && x.ProjectID == projectID).OrderByDescending(x => x.STT).ToList();
                return Ok(ApiResponseFactory.Success(dtProject, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-project-request")]
        public async Task<IActionResult> GetProjectRequest(int projectID)
        {
            try
            {
                var dtProjectRequest = SQLHelper<object>.ProcedureToList("spGetProjectRequest",
                    new string[] { "@ProjectID" },
                    new object[] { projectID });
                var projectRequest = dtProjectRequest?.FirstOrDefault();
                return Ok(ApiResponseFactory.Success(projectRequest, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int projectID)
        {
            try
            {
                var dtProjectSolutions = SQLHelper<object>.ProcedureToList("spGetProjectSolution",
                    new string[] { "@ProjectID" },
                    new object[] { projectID });
                var projectSolutions = dtProjectSolutions?.FirstOrDefault();
                return Ok(ApiResponseFactory.Success(projectSolutions, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpPost("save-data")]
        //public async Task<IActionResult> SaveData([FromBody] ProjectSolution request)
        //{
        //    try
        //    {
        //        if(!_projectSolutionRepo.Validate(request, out string message))
        //        {
        //                return BadRequest(ApiResponseFactory.Fail(null, message));
        //        }
        //        if (request.ID > 0)
        //        {
        //            await _projectSolutionRepo.UpdateAsync(request);
        //            return Ok(ApiResponseFactory.Success(
        //                request,
        //                ""
        //            ));
        //        }
        //        else
        //        {
        //            await _projectSolutionRepo.CreateAsync(request);
        //            return Ok(ApiResponseFactory.Success(request
        //                ,
        //                ""
        //            ));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi lưu giải pháp"));
        //    }
        //}
        [HttpPost("save-data-solution")]
        [RequiresPermission("N13,N1,,N27,N63")]
        public async Task<IActionResult> SaveData([FromBody] ProjectSolutionDTO request)
        {
            try
            {
                string message = "";
                int projectSolutionID = 0;

                // XỬ LÝ DUYỆT (NẾU CÓ THÔNG TIN DUYỆT)
                if (request.ApproveStatus.HasValue && request.IsApproveAction.HasValue)
                {
                    if (!_projectSolutionRepo.ValidateApprove(request.ID, request.IsApproveAction.Value, request.ApproveStatus.Value, out message))
                        return BadRequest(ApiResponseFactory.Fail(null, message));

                    string key = _configuration.GetValue<string>("SessionKey") ?? "";
                    CurrentUser currentUser = HttpContext.Session.GetObject<CurrentUser>(key);
                    var solution = await _projectSolutionRepo.GetByIDAsync(request.ID);

                    //if (solution == null) return NotFound();

                    if (request.ApproveStatus == 1)
                    {
                        solution.IsApprovedPrice = request.IsApproveAction;
                        solution.EmployeeApprovedPriceID = currentUser.EmployeeID;
                    }
                    else if (request.ApproveStatus == 2)
                    {
                        solution.IsApprovedPO = request.IsApproveAction;
                        solution.EmployeeApprovedPOID = currentUser.EmployeeID;
                    }

                    await _projectSolutionRepo.UpdateAsync(solution);
                    return Ok(ApiResponseFactory.Success(solution, "Duyệt thành công"));
                }
                if (!_projectSolutionRepo.Validate(request, out message))
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                // XỬ LÝ LƯU DỮ LIỆU THÔNG THƯỜNG
                if (request.ID > 0)
                {
                    await _projectSolutionRepo.UpdateAsync(request);
                    projectSolutionID = request.ID;
                }
                else
                {
                    await _projectSolutionRepo.CreateAsync(request);
                    projectSolutionID = request.ID;
                }

                ///lohic them file
                if (request.projectSolutionFile?.Count > 0)
                {
                    foreach (var item in request.projectSolutionFile)
                    {
                        if (item.ID > 0)
                        {

                            await _projectSolutionFilRepo.UpdateAsync(item);
                        }
                        else
                        {
                            item.ProjectSolutionID = projectSolutionID;
                            await _projectSolutionFilRepo.CreateAsync(item);
                        }
                    }
                }
                if (request.deletedFileID?.Count > 0)
                {
                    foreach (var item in request.deletedFileID)
                    {
                        var data = _projectSolutionFilRepo.GetByID(item);
                        data.IsDeleted = true;
                        await _projectSolutionFilRepo.UpdateAsync(data);
                    }
                }

                return Ok(ApiResponseFactory.Success(request, "Lưu dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi xử lý dữ liệu"));
            }
        }
        [HttpGet("get-solution-code")]
        public async Task<IActionResult> GetSolutionCode(int projectRequestId)
        {
            try
            {
                string code = _projectSolutionRepo.GetSolutionCode(projectRequestId);
                return Ok(ApiResponseFactory.Success(code, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi lấy mã giải pháp"));
            }
        }

        //[HttpPost("approve")]
        //public async Task<IActionResult> ApproveSolution(int id, bool isApproved, int status)
        //{

        //    if (!_projectSolutionRepo.ValidateApprove(id, isApproved, status, out string message))
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(null, message));
        //    }
        //    var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        //    var currentUser = ObjectMapper.GetCurrentUser(claims);
        //    //if (currentUser == null)
        //    //{
        //    //    return Unauthorized(ApiResponseFactory.Fail(null, "Bạn không có quyền thực hiện thao tác này"));
        //    //}
        //    var solution = await _projectSolutionRepo.GetByIDAsync(id);
        //    if (solution == null) return NotFound();

        //    if (status == 1)
        //    {
        //        solution.IsApprovedPrice = isApproved;
        //        solution.EmployeeApprovedPriceID = currentUser.EmployeeID;
        //    }
        //    else if (status == 2)
        //    {
        //        solution.IsApprovedPO = isApproved;
        //        solution.EmployeeApprovedPOID = currentUser.EmployeeID;
        //    }

        //   await _projectSolutionRepo.UpdateAsync(solution);
        //    return Ok(ApiResponseFactory.Success(solution, "Duyệt thành công"));
        //}

        #region dành cho yêu cầu - gaiir pháp
        [HttpGet("get-project-request2")]
        public async Task<IActionResult> GetProjectRequest2(int projectID, string? keyword)
        {
            try
            {
                var dtProjectRequest = SQLHelper<object>.ProcedureToList("spGetProjectRequest",
                    new string[] { "@ProjectID", "@Keyword" },
                    new object[] { projectID, keyword ?? "" });
                var projectRequest = dtProjectRequest?.FirstOrDefault();
                return Ok(ApiResponseFactory.Success(projectRequest, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-project-solution")]
        public async Task<IActionResult> GetSolution(int projectID, int projectRequestID)
        {
            try
            {
                var dtProjectSolutions = SQLHelper<object>.ProcedureToList("spGetProjectSolution",
                    new string[] { "@ProjectID", "ProjectRequestID" },
                    new object[] { projectID, projectRequestID });
                var projectSolutions = dtProjectSolutions?.FirstOrDefault();
                return Ok(ApiResponseFactory.Success(projectSolutions, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-solution-file")]
        public async Task<IActionResult> GetSolutionFile(int projectSolutionID)
        {
            try
            {
                List<ProjectSolutionFile> lstFile = _projectSolutionFilRepo.GetAll(x => x.ProjectSolutionID == projectSolutionID && x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(lstFile, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-request-file")]
        public async Task<IActionResult> GetRequestFile(int projectRequestID)
        {
            try
            {
                var lstFile = _projectRequestFileRepo.GetAll(x => x.ProjectRequestID == projectRequestID && x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(lstFile, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-request")]
        [RequiresPermission("N13,N1,N63")]
        public async Task<IActionResult> SaveDataRequest([FromBody] ProjectRequestDTO request)
        {
            try
            {
                string message = "";
                int projectRequestID = 0;

                if (!_projectRequestRepo.Validate(request, out message))
                {
                    return Ok(new { status = 2, message });
                }
                // XỬ LÝ LƯU DỮ LIỆU THÔNG THƯỜNG
                if (request.ID > 0)
                {
                    await _projectRequestRepo.UpdateAsync(request);
                    projectRequestID = request.ID;
                }
                else
                {
                    await _projectRequestRepo.CreateAsync(request);
                    projectRequestID = request.ID;
                }

                ///lohic them file
                if (request.projectRequestFile?.Count > 0)
                {
                    foreach (var item in request.projectRequestFile)
                    {
                        if (item.ID > 0)
                        {

                            await _projectRequestFileRepo.UpdateAsync(item);
                        }
                        else
                        {
                            item.ProjectRequestID = projectRequestID;
                            await _projectRequestFileRepo.CreateAsync(item);
                        }
                    }
                }
                if (request.deletedFileID?.Count > 0)
                {
                    foreach (var item in request.deletedFileID)
                    {
                        var data = _projectRequestFileRepo.GetByID(item);
                        data.IsDeleted = true;
                        await _projectRequestFileRepo.UpdateAsync(data);
                    }
                }

                return Ok(ApiResponseFactory.Success(request, "Lưu dữ liệu thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, "Lỗi khi xử lý dữ liệu"));
            }
        }
        #endregion
    }
}