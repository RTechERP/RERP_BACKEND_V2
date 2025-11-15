using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;

namespace RERPAPI.Controllers.ProjectManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectReportController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult GetProjectReport(DateTime? startDate = null, DateTime? endDate = null, 
            string projectType = "", int departmentId = 0, int employeeId = 0)
        {
            try
            {
                // Set default dates if not provided - theo pattern trong projectSumary.txt
                var fromDate = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var toDate = endDate ?? fromDate.AddMonths(1).AddDays(-1);

                var result = SQLHelper<object>.ProcedureToList(
                    "spGetProjectReport",
                    new[] { "@StartDate", "@EndDate", "@ProjectType" },
                    new object[] { fromDate, toDate, projectType ?? "" }
                );

                // Lấy dữ liệu từ các bảng khác nhau như trong projectSumary.txt
                var chartData1 = SQLHelper<object>.GetListData(result, 0);
                var chartData2 = SQLHelper<object>.GetListData(result, 1);
                var quarterData1 = SQLHelper<object>.GetListData(result, 2);
                var quarterData2 = SQLHelper<object>.GetListData(result, 3);

                return Ok(ApiResponseFactory.Success(new
                {
                    ChartData1 = chartData1,
                    ChartData2 = chartData2,
                    QuarterData1 = quarterData1,
                    QuarterData2 = quarterData2
                }, "Lấy dữ liệu báo cáo dự án thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("")]
        public IActionResult GetProjectReportPost([FromBody] ProjectReportFilterDTO filter)
        {
            try
            {
                var result = SQLHelper<object>.ProcedureToList(
                    "spGetProjectReport",
                    new[] { "@StartDate", "@EndDate", "@ProjectType" },
                    new object[] { filter.StartDate, filter.EndDate, filter.ProjectType ?? "" }
                );

                // Xử lý dữ liệu như trong projectSumary.txt
                var chartData1 = SQLHelper<object>.GetListData(result, 0);
                var chartData2 = SQLHelper<object>.GetListData(result, 1);
                var quarterData1 = SQLHelper<object>.GetListData(result, 2);
                var quarterData2 = SQLHelper<object>.GetListData(result, 3);

                return Ok(ApiResponseFactory.Success(new
                {
                    ChartData1 = chartData1,
                    ChartData2 = chartData2,
                    QuarterData1 = quarterData1,
                    QuarterData2 = quarterData2
                }, "Lấy dữ liệu báo cáo dự án thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("summary")]
        public IActionResult GetProjectSummary(DateTime? startDate = null, DateTime? endDate = null, 
            string projectType = "", int departmentId = 0)
        {
            try
            {
                var fromDate = startDate ?? DateTime.Now.AddMonths(-1);
                var toDate = endDate ?? DateTime.Now;

                var result = SQLHelper<object>.ProcedureToList(
                    "spGetProjectSummary",
                    new[] { "@StartDate", "@EndDate", "@ProjectType", "@DepartmentID" },
                    new object[] { fromDate, toDate, projectType ?? "", departmentId }
                );

                return Ok(ApiResponseFactory.Success(result, "Lấy tổng hợp báo cáo dự án thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("by-status")]
        public IActionResult GetProjectByStatus(DateTime? startDate = null, DateTime? endDate = null, 
            string status = "", int departmentId = 0)
        {
            try
            {
                var fromDate = startDate ?? DateTime.Now.AddMonths(-1);
                var toDate = endDate ?? DateTime.Now;

                var result = SQLHelper<object>.ProcedureToList(
                    "spGetProjectByStatus",
                    new[] { "@StartDate", "@EndDate", "@Status", "@DepartmentID" },
                    new object[] { fromDate, toDate, status ?? "", departmentId }
                );

                return Ok(ApiResponseFactory.Success(result, "Lấy dữ liệu dự án theo trạng thái thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("departments")]
        public IActionResult GetDepartments()
        {
            try
            {
                var result = SQLHelper<object>.ProcedureToList(
                    "spGetDepartments",
                    new string[] { },
                    new object[] { }
                );

                return Ok(ApiResponseFactory.Success(result, "Lấy danh sách phòng ban thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("employees")]
        public IActionResult GetEmployees(int departmentId = 0)
        {
            try
            {
                var result = SQLHelper<object>.ProcedureToList(
                    "spGetEmployeesByDepartment",
                    new[] { "@DepartmentID" },
                    new object[] { departmentId }
                );

                return Ok(ApiResponseFactory.Success(result, "Lấy danh sách nhân viên thành công!"));
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
                var result = SQLHelper<object>.ProcedureToList(
                    "spGetProjectTypes",
                    new string[] { },
                    new object[] { }
                );

                return Ok(ApiResponseFactory.Success(result, "Lấy danh sách loại dự án thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("export")]
        public IActionResult ExportProjectReport(DateTime? startDate = null, DateTime? endDate = null, 
            string projectType = "", int departmentId = 0, int employeeId = 0, string format = "excel")
        {
            try
            {
                var fromDate = startDate ?? DateTime.Now.AddMonths(-1);
                var toDate = endDate ?? DateTime.Now;

                var result = SQLHelper<object>.ProcedureToList(
                    "spGetProjectReport",
                    new[] { "@StartDate", "@EndDate", "@ProjectType", "@DepartmentID", "@EmployeeID" },
                    new object[] { fromDate, toDate, projectType ?? "", departmentId, employeeId }
                );

                // Here you would implement the actual export logic based on format
                return Ok(ApiResponseFactory.Success(new 
                {
                    Data = result,
                    ExportInfo = new
                    {
                        StartDate = fromDate,
                        EndDate = toDate,
                        ProjectType = projectType,
                        DepartmentId = departmentId,
                        EmployeeId = employeeId,
                        Format = format,
                        ExportDate = DateTime.Now
                    }
                }, $"Xuất báo cáo {format.ToUpper()} thành công!"));
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
                // Load project types như trong projectSumary.txt
                var projectTypes = SQLHelper<object>.FindAll();
                
                return Ok(ApiResponseFactory.Success(projectTypes, "Lấy danh sách loại dự án thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }

    public class ProjectReportFilterDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ProjectType { get; set; }
        public int DepartmentId { get; set; }
        public int EmployeeId { get; set; }
    }
}