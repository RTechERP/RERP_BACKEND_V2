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

        public ProjectWorkerController(ProjectTypeRepo projectTypeRepo)
        {
            this.projectTypeRepo = projectTypeRepo;
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
        public async Task<IActionResult> GetProjectWorker(ProjectWorkerParamRequest param)
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
        //phần chi tiết
        [HttpGet("get-project-type")]
        public async Task<IActionResult> GetProjectType()
        {
            try
            {
                var rs = projectTypeRepo.GetAll(x=>x.IsDeleted ==false);
                return Ok(ApiResponseFactory.Success(rs, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
