using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers.Old
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthorize]
    public class ProjectSolutionFileController : ControllerBase
    {
        ProjectSolutionFileRepo _repo= new ProjectSolutionFileRepo();
        [HttpGet("get-all")]
        public IActionResult GetAll(int projectSolutionID)
        {
            try
            {
                List<ProjectSolutionFile> dtAll  = _repo.GetAll(x => x.ProjectSolutionID == projectSolutionID );
                return Ok(ApiResponseFactory.Success(dtAll, ""));
                
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex,ex.Message));
            }
        }
    }
}
