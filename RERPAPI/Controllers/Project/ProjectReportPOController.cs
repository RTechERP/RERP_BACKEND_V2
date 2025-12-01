using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;

namespace RERPAPI.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectReportPOController : ControllerBase
    {
        [HttpGet("ProjectReport")]
        public IActionResult GetProjectReport(DateTime startDate, DateTime endDate, string projectType = "")
        {
            try
            {
                var rawData = SQLHelper<object>.ProcedureToList(
                    "spGetProjectReport",
                    new[] { "@StartDate", "@EndDate", "@ProjectType" },
                    new object[] { startDate, endDate, projectType ?? "" }
                );

                var monthlyNoPO = SQLHelper<object>.GetListData(rawData, 0);
                var monthlyWithPO = SQLHelper<object>.GetListData(rawData, 1);
                var quarterlyNoPO = SQLHelper<object>.GetListData(rawData, 2);
                var quarterlyWithPO = SQLHelper<object>.GetListData(rawData, 3);

                return Ok(ApiResponseFactory.Success(new
                {
                    MonthlyNoPO = monthlyNoPO,
                    MonthlyWithPO = monthlyWithPO,
                    QuarterlyNoPO = quarterlyNoPO,
                    QuarterlyWithPO = quarterlyWithPO
                }, "Lấy dữ liệu báo cáo thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("ProjectReport")]
        public IActionResult GetProjectReportPost([FromBody] ProjectReportRequestDTO request)
        {
            try
            {
                var rawData = SQLHelper<object>.ProcedureToList(
                    "spGetProjectReport",
                    new[] { "@StartDate", "@EndDate", "@ProjectType" },
                    new object[] { request.StartDate, request.EndDate, request.ProjectType ?? "" }
                );

                var monthlyNoPO = SQLHelper<object>.GetListData(rawData, 0);
                var monthlyWithPO = SQLHelper<object>.GetListData(rawData, 1);
                var quarterlyNoPO = SQLHelper<object>.GetListData(rawData, 2);
                var quarterlyWithPO = SQLHelper<object>.GetListData(rawData, 3);

                return Ok(ApiResponseFactory.Success(new
                {
                    MonthlyNoPO = monthlyNoPO,
                    MonthlyWithPO = monthlyWithPO,
                    QuarterlyNoPO = quarterlyNoPO,
                    QuarterlyWithPO = quarterlyWithPO,
                    ChartData = new
                    {
                        MonthlyNoPOChart = ProcessChartData(monthlyNoPO),
                        MonthlyWithPOChart = ProcessChartData(monthlyWithPO),
                        QuarterlyNoPOChart = ProcessChartData(quarterlyNoPO),
                        QuarterlyWithPOChart = ProcessChartData(quarterlyWithPO)
                    }
                }, "Lấy dữ liệu báo cáo thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("chart-data")]
        public IActionResult GetChartData(DateTime startDate, DateTime endDate, string projectType = "", string chartType = "monthly")
        {
            try
            {
                var rawData = SQLHelper<object>.ProcedureToList(
                    "spGetProjectReport",
                    new[] { "@StartDate", "@EndDate", "@ProjectType" },
                    new object[] { startDate, endDate, projectType ?? "" }
                );

                object chartData = null;

                switch (chartType.ToLower())
                {
                    case "monthly-nopo":
                        chartData = ProcessChartData(SQLHelper<object>.GetListData(rawData, 0));
                        break;
                    case "monthly-withpo":
                        chartData = ProcessChartData(SQLHelper<object>.GetListData(rawData, 1));
                        break;
                    case "quarterly-nopo":
                        chartData = ProcessChartData(SQLHelper<object>.GetListData(rawData, 2));
                        break;
                    case "quarterly-withpo":
                        chartData = ProcessChartData(SQLHelper<object>.GetListData(rawData, 3));
                        break;
                    default:
                        return BadRequest(ApiResponseFactory.Fail(null, "Loại biểu đồ không hợp lệ!"));
                }

                return Ok(ApiResponseFactory.Success(chartData, "Lấy dữ liệu biểu đồ thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("project-types")]
        public IActionResult GetProjectTypes()
        {
            try
            {
                var projectTypes = SQLHelper<object>.ProcedureToList(
                    "spGetProjectTypes",
                    new string[] { },
                    new object[] { }
                );

                return Ok(ApiResponseFactory.Success(projectTypes, "Lấy danh sách loại dự án thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("export-excel")]
        public IActionResult ExportToExcel(DateTime startDate, DateTime endDate, string projectType = "")
        {
            try
            {
                var rawData = SQLHelper<object>.ProcedureToList(
                    "spGetProjectReport",
                    new[] { "@StartDate", "@EndDate", "@ProjectType" },
                    new object[] { startDate, endDate, projectType ?? "" }
                );

                var monthlyNoPO = SQLHelper<object>.GetListData(rawData, 0);
                var monthlyWithPO = SQLHelper<object>.GetListData(rawData, 1);
                var quarterlyNoPO = SQLHelper<object>.GetListData(rawData, 2);
                var quarterlyWithPO = SQLHelper<object>.GetListData(rawData, 3);

                // Here you would implement Excel export logic
                // For now, return the data that would be exported
                return Ok(ApiResponseFactory.Success(new
                {
                    MonthlyNoPO = monthlyNoPO,
                    MonthlyWithPO = monthlyWithPO,
                    QuarterlyNoPO = quarterlyNoPO,
                    QuarterlyWithPO = quarterlyWithPO,
                    ExportInfo = new
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        ProjectType = projectType,
                        ExportDate = DateTime.Now
                    }
                }, "Dữ liệu xuất Excel!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private object ProcessChartData(object rawData)
        {
            // Process raw data for chart display
            // This would depend on the actual structure of your data
            // For now, return the raw data
            return rawData;
        }
    }

    public class ProjectReportRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ProjectType { get; set; }
    }
}
