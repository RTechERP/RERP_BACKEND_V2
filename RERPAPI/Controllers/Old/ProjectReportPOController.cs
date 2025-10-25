using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectReportPOController : ControllerBase
    {
        [HttpGet("ProjectReport")]
        public IActionResult GetProjectReport(DateTime startDate, DateTime endDate, string projectType)
        {
            try
            {
                var rawData = SQLHelper<object>.ProcedureToList(
                    "spGetProjectReport",
                    new[] { "@StartDate", "@EndDate", "@ProjectType" },
                    new object[] { startDate, endDate, projectType }
                );

                var monthlyNoPO = SQLHelper<object>.GetListData(rawData, 0);
                var monthlyWithPO = SQLHelper<object>.GetListData(rawData, 1);
                var quarterlyNoPO = SQLHelper<object>.GetListData(rawData, 2);
                var quarterlyWithPO = SQLHelper<object>.GetListData(rawData, 3);

                return Ok(new
                {
                    status = 1,
                    data = new
                    {
                        MonthlyNoPO = monthlyNoPO,
                        MonthlyWithPO = monthlyWithPO,
                        QuarterlyNoPO = quarterlyNoPO,
                        QuarterlyWithPO = quarterlyWithPO
                    }
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
