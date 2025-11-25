using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.Param;
using RERPAPI.Repo.GenericEntity;
using System.Linq.Expressions;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectWorkerVersionController : ControllerBase
    {
        private ProjectWorkerVersionRepo _repo;
        public ProjectWorkerVersionController(ProjectWorkerVersionRepo repo)
        {
            _repo = repo;
        }
       

        [HttpPost("approved-active")]
        public async Task<IActionResult> ApprovedActive([FromBody] ApprovedActiveRequestParam request)
        {
            try
            {
                var projectWorkerVersion = _repo.GetByID(request.ProjectWorkerVersionID);
                projectWorkerVersion.IsActive = request.IsActive;

                await _repo.UpdateAsync(projectWorkerVersion);

                return Ok(ApiResponseFactory.Success(null, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
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
