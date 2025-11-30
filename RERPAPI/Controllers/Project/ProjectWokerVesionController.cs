using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
                    if(item.StatusVersion == 2)
                    {
                        var check = _projectWorkerVersionRepo.GetAll(x => x.ProjectSolutionID == item.ProjectSolutionID && x.StatusVersion == 2 && x.IsDeleted == false && x.ProjectTypeID == item.ProjectTypeID);
                        if (check.Count > 0)
                        {
                            return Ok(new { status = 2, message = $"Danh mục vừa chọn đã có phiên bản Po" });
                        }
                    }

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
