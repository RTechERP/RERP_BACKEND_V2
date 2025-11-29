using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectTypeReportController : ControllerBase
    {
        [HttpGet("projecttypereport")]
        public IActionResult GetProjectTypeReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                var rawData = SQLHelper<object>.ProcedureToList(
                    "spGetProjectTypeReport",
                    new[] { "@StartDate", "@EndDate" },
                    new object[] { startDate, endDate }
                );
                var reportData = SQLHelper<object>.GetListData(rawData, 0);

                return Ok(new
                {
                    status = 1,
                    data = reportData
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
        [HttpGet("projectbytype")]
        public IActionResult GetProjectByType(int projectTypeId, DateTime dateStart, DateTime dateEnd, int status)
        {
            try
            {
                var rawData = SQLHelper<object>.ProcedureToList(
                    "spGetProjectByType",
                    new[] { "@ProjectTypeID", "@DateStart", "@DateEnd", "@Status" },
                    new object[] { projectTypeId, dateStart, dateEnd, status }
                );
                var projectData = SQLHelper<object>.GetListData(rawData, 0);

                return Ok(new
                {
                    status = 1,
                    data = projectData
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 0,
                    message = ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
