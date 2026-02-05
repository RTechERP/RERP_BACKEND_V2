using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Param;
using RERPAPI.Model.Param.HRM.VehicleManagement;
using RERPAPI.Repo.GenericEntity;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity.Film;

namespace RERPAPI.Controllers.HRM.DailyReportHR
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyReportHrController : ControllerBase
    {
        public DailyReportLXCP _dailyReportHRRepo;
        public FilmManagementDetailRepo _filmDetail;
        public DailyReportHrController(DailyReportLXCP dailyReportHRRepo,FilmManagementDetailRepo filmDetail)
        {
            _dailyReportHRRepo = dailyReportHRRepo;
            _filmDetail = filmDetail;
        }

        [HttpPost("get-daily-report-hr")]
        [RequiresPermission("N42,N2,N1")]
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

                var ds = startDate.AddHours(00).AddMinutes(00).AddSeconds(00); // 00:00:00
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
                        dataDriver,
                        hrAll
                    },
                    "Lấy dữ liệu thành công"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [RequiresPermission("N1,N44")]
        [HttpGet("get-film-detail")]
        public IActionResult GetFilmDetail()
        {
            try
            {
                var filmDetail = SQLHelper<dynamic>.ProcedureToList("spGetFilmManagementDetail",
                                                    new string[] { "@FilmManagementID", },
                                                    new object[] { 0 });
                var data = SQLHelper<dynamic>.GetListData(filmDetail, 0);
                return Ok(ApiResponseFactory.Success(data, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpGet("get-data-by-id")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var dailyReportLXCP = _dailyReportHRRepo.GetByID(id);
                return Ok(ApiResponseFactory.Success(dailyReportLXCP, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [HttpPost("save-report-hr")]
        public async Task<IActionResult> SaveReportHR([FromBody] List<Model.Entities.DailyReportHR> request)
        {
            try
            {
                var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                var currentUser = ObjectMapper.GetCurrentUser(claims);
                // int userId = currentUser.ID; // Nếu cần dùng ID int
                if (request == null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu gửi lên không hợp lệ!"));
                }
                foreach (var item in request)
                {
                    if (item.IsDeleted == true)
                    {
                        var dataDelete = _dailyReportHRRepo.GetByID(item.ID);
                        dataDelete.IsDeleted = true;
                        await _dailyReportHRRepo.UpdateAsync(dataDelete);
                        return Ok(ApiResponseFactory.Success(null, "Xóa báo cáo thành công"));
                    }
                   
                }
                // 1. Kiểm tra request null hoặc empty
                if (request == null || request.Count == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Danh sách báo cáo không được rỗng!"));
                }

                if (!_dailyReportHRRepo.ValidateDailyReportHRList(request, out string validationMessage, currentUser.EmployeeID))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, validationMessage));
                }


                foreach (var item in request)
                {
                    // --- LOGIC TÍNH TOÁN (Từ RTC Web) ---

                    // 1. Trường hợp Cắt phim (Có chọn công việc phim)
                    if (item.FilmManagementDetailID.HasValue && item.FilmManagementDetailID > 0)
                    {
                        // Lấy thông tin chi tiết phim để lấy Năng suất trung bình (PerformanceAvg)
                        var filmDetail = _filmDetail.GetAll(x=>x.FilmManagementID == item.FilmManagementDetailID).FirstOrDefault();
                        decimal performanceAVG = filmDetail != null ? (decimal)filmDetail.PerformanceAVG : 0;

                        // Tính Năng suất thực tế = Thời gian / Số lượng
                        item.PerformanceActual = item.PerformanceActual ?? 0;
                        // Tính Tỷ lệ = Năng suất TB / Năng suất thực tế
                        // (Năng suất thực tế càng NHỎ tức là làm càng NHANH -> Tỷ lệ càng CAO)
                        item.Percentage = item.Percentage ?? 0;
                    }
                    // 2. Trường hợp Lái xe (Không chọn công việc phim)
                    else
                    {
                        item.KmNumber = item.KmNumber ?? 0;
                        item.TotalLate = item.TotalLate ?? 0;
                        item.TotalTimeLate = item.TotalTimeLate ?? 0;
                    }
                    item.EmployeeID = currentUser.EmployeeID; // Gán ID nhân viên đang đăng nhập
                    ///item.Quantity = Convert.ToInt32(item.Quantity);
                    if (item.ID > 0)
                    {                    
                        await _dailyReportHRRepo.UpdateAsync(item);
                    }
                    else
                    {
                        await _dailyReportHRRepo.CreateAsync(item);
                    }
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
