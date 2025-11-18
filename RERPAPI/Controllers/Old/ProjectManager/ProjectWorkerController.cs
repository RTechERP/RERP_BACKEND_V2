using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectWorkerController : ControllerBase
    {
        private readonly ProjectTypeRepo projectTypeRepo;
        private readonly ProjectSolutionFileRepo projectSolutionFileRepo;
        private readonly ProjectWorkerRepo projectWorkerRepo;

        public ProjectWorkerController(ProjectTypeRepo projectTypeRepo, ProjectSolutionFileRepo projectSolutionFileRepo, ProjectWorkerRepo projectWorkerRepo)
        {
            this.projectTypeRepo = projectTypeRepo;
            this.projectSolutionFileRepo = projectSolutionFileRepo;
            this.projectWorkerRepo = projectWorkerRepo;
        }

        //load giải pháp
        [HttpGet("get-solution/{projectID}")]
        public async Task<IActionResult> GetSolution(int projectID)
        {
            try
            {
                var projectSolution = SQLHelper<dynamic>.ProcedureToList("spGetProjectSolution",
                    new string[] { "ProjectID" },
                    new object[] { projectID });
                return Ok(ApiResponseFactory.Success(SQLHelper<dynamic>.GetListData(projectSolution, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //load phiên bản giải pháp
        [HttpGet("get-solution-version/{projectSolutionId}")]
        public async Task<IActionResult> GetSolutionVersion(int projectSolutionId)
        {
            try
            {
                var projectSolutionVersion = SQLHelper<dynamic>.ProcedureToList("spGetProjectWorkerVersion_New",
                    new string[] { "ProjectSolutionID", "@StatusVersion" },
                    new object[] { projectSolutionId, 1 });
                return Ok(ApiResponseFactory.Success(SQLHelper<dynamic>.GetListData(projectSolutionVersion, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //load phiên bản po
        [HttpGet("get-version-po/{projectSolutionId}")]
        public async Task<IActionResult> GetVersionPO(int projectSolutionId)
        {
            try
            {
                var projectSolutionVersion = SQLHelper<dynamic>.ProcedureToList("spGetProjectWorkerVersion_New",
                    new string[] { "ProjectSolutionID", "@StatusVersion" },
                    new object[] { projectSolutionId, 2 });
                return Ok(ApiResponseFactory.Success(SQLHelper<dynamic>.GetListData(projectSolutionVersion, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //load nhân công 
        [HttpPost("get-project-worker")]
        public async Task<IActionResult> GetProjectWorker([FromBody] ProjectWorkerParamRequest param)
        {
            try
            {
                var projectWorker = SQLHelper<dynamic>.ProcedureToList("spGetProjectWorker",
                    new string[] { "@ProjectID", "@ProjectWorkerTypeID", "@IsApprovedTBP", "@IsDeleted", "@KeyWord", "@ProjectWorkerVersion" },
                    new object[] { param.projectID, param.projectWorkerTypeID, param.IsApprovedTBP, param.IsDeleted, param.KeyWord, param.versionID });
                return Ok(ApiResponseFactory.Success(SQLHelper<dynamic>.GetListData(projectWorker, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //load file giải pháp
        [HttpGet("get-project-solution-file")]
        public async Task<IActionResult> GetProjectSolutionFile(int projectSolutionID)
        {
            try
            {
                List<ProjectSolutionFile> projectSolutionFile = projectSolutionFileRepo.GetAll(x => x.ProjectSolutionID == projectSolutionID && x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(projectSolutionFile, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //phần chi tiết
        [HttpGet("get-project-type")]
        public async Task<IActionResult> GetProjectType()
        {
            try
            {
                var rs = projectTypeRepo.GetAll(x => x.IsDeleted == false);
                return Ok(ApiResponseFactory.Success(rs, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        //save nhân công
        [HttpPost("save-project-worker")]
        public async Task<IActionResult> SaveProjectWorker([FromBody] List<ProjectWorker> data)
        {
            try
            {
                foreach (var pw in data)
                {
                    if (pw.IsDeleted != true && pw.TT != null )
                    {
                        if (projectWorkerRepo.checkTTExists(pw.TT, pw.ParentID, pw.ID, pw.ProjectWorkerVersionID ?? 0))
                        {
                            return Ok(new
                            {
                                status = 2,
                                message = "TT đã tồn tại, vui lòng kiểm tra lại!"
                            });

                        }
                    }
                        int parentId = projectWorkerRepo.FindParentIdByTT(pw.TT, pw.ProjectWorkerVersionID ?? 0);
                    if (pw.ID > 0)
                    {
                        //pw.UpdatedBy = CurrentUser.UserName;
                        await projectWorkerRepo.UpdateAsync(pw);
                    }
                    else
                    {
                        pw.ParentID = parentId;
                        pw.IsApprovedTBP = false;
                        pw.StatusVersion = 0;
                        pw.ProjectTypeID = 0;
                        await projectWorkerRepo.CreateAsync(pw);
                        //pw.CreatedBy = CurrentUser.UserName
                    }
                }  
                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
