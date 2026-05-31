using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ProjectJoinSummaryController : ControllerBase
    {
        private CurrentUser _currentUser;
        public ProjectJoinSummaryController(CurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet("get-project-join-summary")]   
        public IActionResult GetProjectJoinSummary(int employeeId, int userId, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetProjectByEmployeeID",
                    new string[] { "@EmployeeID", "@UserIDPriotity", "@DateStart", "@DateEnd" },
                    new object[] { employeeId, currentUser.ID, dateStart, dateEnd });

                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-project-joined")]
        public IActionResult GetProjectJoined(int employeeId)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetProjectEmployeeByEmployeeID",
                    new string[] { "@EmployeeID" },
                    new object[] { employeeId });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("get-employees")]
        public IActionResult GetEmployee(int status)
        {
            try
            {
                List<List<dynamic>> list = SQLHelper<dynamic>.ProcedureToList("spGetEmployee",
                    new string[] { "@Status" },
                    new object[] { 0 });
                var data = SQLHelper<dynamic>.GetListData(list, 0);
                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
