using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamEmployeeProjectController : ControllerBase
    {
        private readonly TeamEmployeeProjectRepo _repo;

        public TeamEmployeeProjectController(TeamEmployeeProjectRepo repo)
        {
            _repo = repo;
        }

        [HttpPost("get-projects-by-employees")]
        public async Task<IActionResult> GetProjectsByEmployees([FromBody] GetProjectsByEmployeesRequest request)
        {
            try
            {
                if (request == null || request.EmployeeIds == null || !request.EmployeeIds.Any())
                {
                    return Ok(ApiResponseFactory.Success(new List<object>(), "Lấy dữ liệu thành công"));
                }

                var data = await _repo.GetProjectsByEmployees(
                    request.EmployeeIds,
                    request.DateFrom,
                    request.DateTo);

                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
