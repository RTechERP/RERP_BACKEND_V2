using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System.Linq.Expressions;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthorize]
    public class ProjectWorkerVersionController : ControllerBase
    {
        ProjectWorkerVersionRepo _repo = new ProjectWorkerVersionRepo();

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
                if ( request.ID > 0)
                {
                    await _repo.UpdateAsync(request);
                }
                else
                {
                    await _repo.CreateAsync(request);
                }

             
                return Ok(ApiResponseFactory.Success(request,""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
