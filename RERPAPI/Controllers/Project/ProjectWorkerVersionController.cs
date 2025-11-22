using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Linq.Expressions;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthorize]
    public class ProjectWorkerVersionController : ControllerBase
    {
        private ProjectWorkerVersionRepo _repo;
        public ProjectWorkerVersionController(ProjectWorkerVersionRepo repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public async Task<IActionResult> SaveData([FromBody] ProjectWorkerVersion request)
        {
            try
            {
                string message = "";
                if (!_repo.Validate(request, out message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (request.ID > 0)
                {
                    await _repo.UpdateAsync(request);
                }
                else
                {
                    await _repo.CreateAsync(request);
                }


                return Ok(ApiResponseFactory.Success(request, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
