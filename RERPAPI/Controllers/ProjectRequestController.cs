using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectRequestController : ControllerBase
    {
        ProjectRequestRepo _projectRequestRepo = new ProjectRequestRepo();
        [HttpGet("get-all")]
        public IActionResult GetAll(int projectID)
        {
            try
            {
                List<ProjectRequest> dtProjectRequests = _projectRequestRepo.GetAll(x => x.ProjectID == projectID).ToList();
                return Ok(ApiResponseFactory.Success(
                           dtProjectRequests,
                           ""
                       ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(
                           ex,
                           ex.Message
                       ));
            }
        }
        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProjectRequest request)
        {
            try
            {
                if (request.ID > 0)
                {
                    List<ProjectRequest> project = _projectRequestRepo.GetAll(x => x.ID != request.ID && x.CodeRequest == request.CodeRequest && x.ProjectID == request.ProjectID);
                    await _projectRequestRepo.UpdateAsync(request);
                    return Ok(ApiResponseFactory.Success(
                        request,
                        ""
                    ));
                }
                else
                {
                    request.CodeRequest = _projectRequestRepo.GetRequestCode(request.ProjectID ?? 0);
                    await _projectRequestRepo.CreateAsync(request);
                    return Ok(ApiResponseFactory.Success(request
                        ,
                        ""
                    ));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(
                           ex,
                           ex.Message
                       ));
            }
        }

    }
}