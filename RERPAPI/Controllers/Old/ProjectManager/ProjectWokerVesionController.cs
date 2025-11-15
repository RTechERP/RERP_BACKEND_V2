using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Old.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectWokerVesionController : ControllerBase
    {
        //nhân công dự án
        private readonly ProjectWorkerVersionRepo _projectWorkerVersionRepo;
        public ProjectWokerVesionController(
          ProjectWorkerVersionRepo projectWorkerVersionRepo
      )
        {
            _projectWorkerVersionRepo = projectWorkerVersionRepo;
        }

        [HttpPost("save-worker-version")]
        public async Task<IActionResult> SaveWorkerVersion([FromBody] ProjectWorkerVersion item)
        {
            try
            {
                string message = "";
                if (!_projectWorkerVersionRepo.Validate(item, out message))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, message));
                }
                if (item.ID > 0)
                {
                    await _projectWorkerVersionRepo.UpdateAsync(item);
                }
                else
                {
                    await _projectWorkerVersionRepo.CreateAsync(item);
                }
                return Ok(ApiResponseFactory.Success(item, "Lưu thành công"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-project-solution-cbb/{projectID}")]
        public async Task<IActionResult> GetProjectSolution(int projectID)
        {
            try
            {
                var result = SQLHelper<object>.ProcedureToList("spGetProjectSolution", new string[] { "@ProjectID" }, new object[] { projectID });
                return Ok(ApiResponseFactory.Success(SQLHelper<object>.GetListData(result, 0), "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

    }
}
