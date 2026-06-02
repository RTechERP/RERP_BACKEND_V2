using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectItemLateController : ControllerBase
    {
        #region load dữ liệu chi tiết

        [HttpGet("get-data")]
        public async Task<IActionResult> getdata(int userId, int projectId, int departmentId, DateTime dateStart, DateTime dateEnd, string? keyword)
        {
            try
            {
                dateStart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                dateEnd = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
                var data = SQLHelper<object>.ProcedureToList("spGetProjectItemLate",
                new string[] { "@FilterText", "@UserID", "@ProjectID", "@DepartmentID", "@StartDate", "@EndDate" },
                new object[] { keyword ?? "", userId, projectId, departmentId, dateStart, dateEnd });

                var dt = SQLHelper<object>.GetListData(data, 0);
                return Ok(ApiResponseFactory.Success(dt, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        #endregion load dữ liệu chi tiết
    }
}