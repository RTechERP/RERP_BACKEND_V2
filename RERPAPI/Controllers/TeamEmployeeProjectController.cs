using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.DTO.HRM;
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

        [Authorize]
        [HttpGet("get-employees")]
        public IActionResult GetEmployee(string userTeamIDs, int? departmentid, int? employeeID)
        {
            try
            {
                departmentid = departmentid ?? 0;
                employeeID = employeeID ?? 0;
                userTeamIDs = userTeamIDs ?? "";

                var data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployee",
                                                new string[] { "@DepartmentID", "@Status", "@ID" },
                                                new object[] { departmentid, 0, employeeID });
                if (!string.IsNullOrEmpty(userTeamIDs) && employeeID <= 0)
                {
                    data = SQLHelper<EmployeeCommonDTO>.ProcedureToListModel("spGetEmployeeByTeamID_New",
                                                new string[] { "@TeamIDs" },
                                                new object[] { userTeamIDs });
                }
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
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
