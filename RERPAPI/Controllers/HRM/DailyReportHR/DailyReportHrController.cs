using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Param;
using RERPAPI.Model.Param.HRM.VehicleManagement;

namespace RERPAPI.Controllers.HRM.DailyReportHR
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyReportHrController : ControllerBase
    {
        [HttpPost("get-daily-report-hr")]
        public IActionResult GetDailyReportHr([FromBody] DailyReportHrRequestParam request)
        {
            try
            {
                // Hôm nay
                var today = DateTime.Today;

                // Đầu tuần là Thứ 2
                int diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
                var weekStart = today.AddDays(-diff).Date;    // Thứ 2
                var weekEnd = weekStart.AddDays(6).Date;      // Chủ nhật

                // Nếu null thì dùng tuần hiện tại
                var startDate = (request.dateStart ?? weekStart).Date;
                var endDate = (request.dateEnd ?? weekEnd).Date;

                var ds = startDate; // 00:00:00
                var de = endDate.AddHours(23).AddMinutes(59).AddSeconds(59); // 23:59:59

                var keyword = (request.keyword ?? string.Empty).Trim();

                var dataTech = SQLHelper<object>.ProcedureToList(
                    "spGetDailyReportTechnical",
                    new[] { "@DateStart", "@DateEnd", "@UserID", "@Keyword", "@DepartmentID" },
                    new object[] { ds, de, request.userID, keyword, 6 }
                );


                var technical = SQLHelper<object>.GetListData(dataTech, 0);
                var dataHr = SQLHelper<object>.ProcedureToList(
                    "spGetDailyReportHR",
                    new[] { "@DateStart", "@DateEnd", "@Keyword", "@EmployeeID" },
                    new object[] { ds, de, keyword, request.employeeID }
                );

                var hrAll = SQLHelper<object>.GetListData(dataHr, 0);

                var dataFilm = hrAll
                    .Where(x =>
                    {
                        int? chucVu = (x.ChucVuHDID is int cv) ? cv : (int?)null;
                        if (!chucVu.HasValue) return false;
                        return chucVu.Value == 7 || chucVu.Value == 72;
                    })
                    .ToList();

                var dataDriver = hrAll
                    .Where(x =>
                    {
                        int? chucVu = (x.ChucVuHDID is int cv) ? cv : (int?)null;
                        if (!chucVu.HasValue) return false;
                        return chucVu.Value == 6;
                    })
                    .ToList();

                return Ok(ApiResponseFactory.Success(
                    new
                    {
                        technical,
                        dataFilm,
                        dataDriver
                    },
                    "Lấy dữ liệu thành công"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
