using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeAttendanceController : ControllerBase
    {
        private readonly EmployeeRepo _employeeRepo;
        private readonly EmployeeAttendanceRepo _employeeAttendanceRepo;
        private readonly DepartmentRepo _departmentRepo;
        public EmployeeAttendanceController(EmployeeRepo employeeRepo, EmployeeAttendanceRepo employeeAttendanceRepo, DepartmentRepo departmentRepo)
        {
            _employeeRepo = employeeRepo;
            _employeeAttendanceRepo = employeeAttendanceRepo;
            _departmentRepo = departmentRepo;
        }
        //[RequiresPermission("N1,N2")]
        [HttpGet("get-department")]
        public IActionResult getDepartment()
        {
            try
            {
                List<Department> departments = _departmentRepo.GetAll().ToList();
                return Ok(ApiResponseFactory.Success(departments, "Lấy dữ liệu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }


        [RequiresPermission("N1,N2")]
        [HttpGet("get-employee-attendance")]
        public IActionResult GetEmployeeAttendance(int departmentID, int employeeID, string? findText, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                DateTime ds = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                DateTime de = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
                var dt = SQLHelper<object>.ProcedureToList("spGetEmployeeAttendance",
                                                   new string[] { "@DepartmentID", "@EmployeeID", "@FindText", "@DateStart", "@DateEnd" },
                                                   new object[] { departmentID, employeeID, findText ?? "", ds, de, });
                var data = SQLHelper<object>.GetListData(dt, 0);

                return Ok(ApiResponseFactory.Success(data, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
        [RequiresPermission("N1,N2")]
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel([FromBody] ImportAttendancePayload payload)
        {
            if (payload == null)
                return BadRequest(ApiResponseFactory.Fail(null, "Payload không được null."));
            if (payload.Rows == null || payload.Rows.Count == 0)
                return BadRequest(ApiResponseFactory.Fail(null, "Payload thiếu 'rows' hoặc 'rows' rỗng."));
            if (payload.DateStart > payload.DateEnd)
                return BadRequest(ApiResponseFactory.Fail(null, "DateStart không được lớn hơn DateEnd."));

            try
            {
                // Khoảng ngày chấm công (00:00 -> 23:59:59) – dùng cho bước xoá & update
                DateTime ds = payload.DateStart.Date;
                DateTime de = payload.DateEnd.Date.AddDays(1).AddSeconds(-1);

                int created = 0, updated = 0;
                var errors = new List<ImportError>();


                var employees = _employeeRepo
      .GetAll()
      .Where(e => !string.IsNullOrWhiteSpace(e.Code))
      .GroupBy(e => e.Code!)
      .ToDictionary(
          g => g.Key!,
          g => g.First()
      );

                if (payload.Overwrite)
                {
                    var employeesInDept = _employeeRepo
                        .GetAll(x => payload.DepartmentId == 0 || x.DepartmentID == payload.DepartmentId)
                        .Where(e => !string.IsNullOrEmpty(e.IDChamCongMoi))
                        .ToList();

                    var empIds = employeesInDept
                        .Select(e => e.IDChamCongMoi)
                        .ToList();

                    if (empIds.Any())
                    {
                        var existingToDelete = _employeeAttendanceRepo.GetAll(ea =>
                            ea.AttendanceDate >= ds &&
                            ea.AttendanceDate <= de &&
                            empIds.Contains(ea.IDChamCongMoi)
                        ).ToList();

                        if (existingToDelete.Any())
                        {
                            _employeeAttendanceRepo.DeleteRange(existingToDelete);
                        }
                    }
                }

                // 3) Nếu không overwrite, load danh sách bản ghi hiện có để UPDATE (option thêm của Web, WinForm không có)
                List<EmployeeAttendance> existingRecords = null;
                if (!payload.Overwrite)
                {
                    existingRecords = _employeeAttendanceRepo.GetAll(ea =>
                        ea.AttendanceDate >= ds && ea.AttendanceDate <= de
                    ).ToList();
                }

                // 4) Xử lý từng dòng payload (giống backgroundWorker1_DoWork)
                foreach (var row in payload.Rows)
                {
                    try
                    {
                        // Bỏ các key kỹ thuật (__rowId, ...)
                        var filtered = row
                            .Where(kvp => !kvp.Key.StartsWith("__"))
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                        int stt = ToInt(GetString(filtered, "STT"));
                        string code = GetString(filtered, "Mã nhân viên", "Mã NV", "Code");
                        DateTime? date = GetDate(filtered, "Ngày", "Date", "AttendanceDate");
                        string sIn = GetString(filtered, "Giờ vào", "CheckIn", "In");
                        string sOut = GetString(filtered, "Giờ ra", "CheckOut", "Out");
                        string dayWeek = GetString(filtered, "Thứ", "DayOfWeek", "Day");

                        // Thiếu dữ liệu bắt buộc
                        if (string.IsNullOrWhiteSpace(code) || !date.HasValue)
                            throw new Exception("Thiếu 'Mã nhân viên' hoặc 'Ngày'.");

                        // WinForm: không lọc ngày trong vòng for → bỏ check date-range ở đây

                        // Không tìm thấy nhân viên theo Code
                        if (!employees.TryGetValue(code, out var emp))
                            throw new Exception($"Không tìm thấy nhân viên Code='{code}'.");

                        if (string.IsNullOrWhiteSpace(emp.IDChamCongMoi))
                            throw new Exception($"Nhân viên Code='{code}' không có IDChamCongMoi.");

                        // Ghép giờ vào/ra với ngày
                        DateTime? inDt = ParseTimeOnDate(date.Value, sIn);
                        DateTime? outDt = ParseTimeOnDate(date.Value, sOut);

                        // Tính công, đi muộn / về sớm, ăn trưa... theo nghiệp vụ cũ
                        var comp = ComputeAttendance(emp.DepartmentID ?? 0, date.Value, inDt, outDt);

                        if (payload.Overwrite)
                        {
                            // OVERWRITE: luôn INSERT mới (giống WinForm – đã xóa trước đó)
                            var newRecord = new EmployeeAttendance
                            {
                                STT = stt,
                                EmployeeID=0,
                                IDChamCongMoi = emp.IDChamCongMoi,
                                AttendanceDate = date.Value,
                                DayWeek = dayWeek ?? "",
                                Interval = "(00:00:00-23:59:00)",
                                CheckIn = inDt?.ToString("HH:mm"),
                                CheckOut = outDt?.ToString("HH:mm"),
                                CheckInDate = inDt,
                                CheckOutDate = outDt,
                                IsLate = comp.IsLate,
                                TimeLate = comp.TimeLate,
                                IsEarly = comp.IsEarly,
                                TimeEarly = comp.TimeEarly,
                                TotalHour = comp.TotalHour,
                                TotalDay = comp.TotalDay,
                                IsLunch = comp.IsLunch,
                                CreatedDate = DateTime.Now,
                                UpdatedDate = DateTime.Now
                            };

                            await _employeeAttendanceRepo.CreateAsync(newRecord);
                            created++;
                        }
                        else
                        {
                            // MODE mở rộng của Web:
                            // Nếu đã có AttendanceDate + IDChamCongMoi thì UPDATE, nếu chưa có thì INSERT
                            var existing = existingRecords?.FirstOrDefault(x =>
                                x.IDChamCongMoi == emp.IDChamCongMoi &&
                                x.AttendanceDate == date.Value
                            );

                            if (existing != null)
                            {
                                existing.STT = stt;
                                existing.EmployeeID = 0;
                                existing.DayWeek = dayWeek ?? "";
                                existing.CheckIn = inDt?.ToString("HH:mm");
                                existing.CheckOut = outDt?.ToString("HH:mm");
                                existing.CheckInDate = inDt;
                                existing.CheckOutDate = outDt;
                                existing.IsLate = comp.IsLate;
                                existing.TimeLate = comp.TimeLate;
                                existing.IsEarly = comp.IsEarly;
                                existing.TimeEarly = comp.TimeEarly;
                                existing.TotalHour = comp.TotalHour;
                                existing.TotalDay = comp.TotalDay;
                                existing.IsLunch = comp.IsLunch;
                                existing.UpdatedDate = DateTime.Now;

                                await _employeeAttendanceRepo.UpdateAsync(existing);
                                updated++;
                            }
                            else
                            {
                                var newRecord = new EmployeeAttendance
                                {
                                    STT = stt,
                                    EmployeeID = 0,
                                    IDChamCongMoi = emp.IDChamCongMoi,
                                    AttendanceDate = date.Value,
                                    DayWeek = dayWeek ?? "",
                                    Interval = "(00:00:00-23:59:00)",
                                    CheckIn = inDt?.ToString("HH:mm"),
                                    CheckOut = outDt?.ToString("HH:mm"),
                                    CheckInDate = inDt,
                                    CheckOutDate = outDt,
                                    IsLate = comp.IsLate,
                                    TimeLate = comp.TimeLate,
                                    IsEarly = comp.IsEarly,
                                    TimeEarly = comp.TimeEarly,
                                    TotalHour = comp.TotalHour,
                                    TotalDay = comp.TotalDay,
                                    IsLunch = comp.IsLunch,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now
                                };

                                await _employeeAttendanceRepo.CreateAsync(newRecord);
                                created++;
                            }
                        }
                    }
                    catch (Exception exRow)
                    {
                        errors.Add(new ImportError
                        {
                            Row = row.TryGetValue("STT", out var sttVal) ? sttVal?.ToString() ?? "?" : "?",
                            Message = exRow.Message
                        });
                    }
                }

                var result = new ImportResult
                {
                    Created = created,
                    Updated = updated,
                    Skipped = errors.Count,
                    Errors = errors
                };

                return Ok(ApiResponseFactory.Success(result, "Import hoàn tất."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, $"Lỗi server: {ex.Message}"));
            }
        }



        [HttpGet("check-existing")]
        public IActionResult CheckExisting(DateTime dateStart, DateTime dateEnd, int departmentId)
        {

            try
            {
                int count = _employeeAttendanceRepo.CheckExisting(dateStart, dateEnd, departmentId);
                var result = new { count = count };
                return Ok(ApiResponseFactory.Success(result, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }

        }

        // ===== Helper methods remain the same =====
        static string GetString(Dictionary<string, object> r, params string[] keys)
        {
            foreach (var k in keys)
                if (r.TryGetValue(k, out var v) && v != null) return v.ToString()!.Trim();
            return string.Empty;
        }

        static int ToInt(string s) => int.TryParse(s, out var n) ? n : 0;

        static DateTime? GetDate(Dictionary<string, object> r, params string[] keys)
        {
            foreach (var k in keys)
            {
                if (!r.TryGetValue(k, out var v) || v == null) continue;
                if (v is DateTime dt) return dt;
                var s = v.ToString();
                if (DateTime.TryParse(s, out var d1)) return d1;
                if (double.TryParse(s, out var dbl))
                {
                    try { return DateTime.FromOADate(dbl); }
                    catch { }
                }
            }
            return null;
        }

        static DateTime? ParseTimeOnDate(DateTime d, string hm)
        {
            if (string.IsNullOrWhiteSpace(hm)) return null;
            if (TimeSpan.TryParse(hm, out var ts))
                return new DateTime(d.Year, d.Month, d.Day, ts.Hours, ts.Minutes, 0);
            if (DateTime.TryParse(hm, out var dtFull))
                return new DateTime(d.Year, d.Month, d.Day, dtFull.Hour, dtFull.Minute, 0);
            if (double.TryParse(hm, out var dbl) && dbl >= 0 && dbl < 1)
            {
                var minutes = (int)Math.Round(dbl * 24 * 60);
                return new DateTime(d.Year, d.Month, d.Day, minutes / 60, minutes % 60, 0);
            }
            return null;
        }

        static (bool IsLate, int TimeLate, bool IsEarly, int TimeEarly, decimal TotalHour, decimal TotalDay, bool IsLunch)
            ComputeAttendance(int departmentId, DateTime date, DateTime? inDt, DateTime? outDt)
        {
            bool isLate = false, isEarly = false, isLunch = false;
            int timeLate = 0, timeEarly = 0;
            decimal totalHour = 0, totalDay = 0;

            if (inDt.HasValue && outDt.HasValue)
            {
                DateTime startAM = new(date.Year, date.Month, date.Day, 8, 1, 0);
                DateTime endAM = new(date.Year, date.Month, date.Day, 12, 0, 0);
                DateTime startPM = new(date.Year, date.Month, date.Day, 13, 31, 0);
                DateTime endPM = new(date.Year, date.Month, date.Day, 17, 30, 0);

                var HCM_SWITCH = new DateTime(2023, 12, 1);
                if (departmentId == 11 && date >= HCM_SWITCH)
                {
                    startPM = new(date.Year, date.Month, date.Day, 13, 1, 0);
                    endPM = new(date.Year, date.Month, date.Day, 17, 0, 0);
                }

                bool workAM = false, workPM = false;

                var difIn = (inDt.Value - startAM).TotalMinutes;
                if (difIn <= 60)
                {
                    isLate = difIn > 0;
                    timeLate = isLate ? (int)Math.Round(difIn) : 0;
                    workAM = true;
                }
                else
                {
                    difIn = (inDt.Value - startPM).TotalMinutes;
                    if (difIn <= 60)
                    {
                        isLate = difIn > 0;
                        timeLate = isLate ? (int)Math.Round(difIn) : 0;
                        workPM = true;
                    }
                }

                var difOut = (outDt.Value - endPM).TotalMinutes;
                if (difOut >= -60)
                {
                    isEarly = difOut < 0;
                    timeEarly = isEarly ? (int)Math.Abs(Math.Round(difOut)) : 0;
                    workPM = true;
                }
                else
                {
                    difOut = (outDt.Value - endAM).TotalMinutes;
                    if (difOut >= -60)
                    {
                        isEarly = difOut < 0;
                        timeEarly = isEarly ? (int)Math.Abs(Math.Round(difOut)) : 0;
                        workAM = true;
                    }
                }

                if (workAM && workPM)
                {
                    var lunch = (startPM - endAM).TotalHours - (1.0 / 60.0);
                    totalHour = (decimal)((outDt.Value - inDt.Value).TotalHours - lunch);
                    totalHour = Math.Round(totalHour, 2);
                    totalDay = totalHour >= 6 ? 1 : 0;
                }
                else
                {
                    totalHour = (decimal)(outDt.Value - inDt.Value).TotalHours;
                    totalHour = Math.Round(totalHour, 2);
                    totalDay = totalHour >= 3 ? 0.5m : 0;
                }
                isLunch = totalHour >= 6 && totalDay > 0.5m;
            }
            return (isLate, timeLate, isEarly, timeEarly, totalHour, totalDay, isLunch);
        }
        [HttpPost("delete-attendance")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var exist = _employeeAttendanceRepo.GetByID(id);
                      if(exist.ID>0)
                    {
                        await _employeeAttendanceRepo.DeleteAsync(exist.ID);
                    }    

                }
                return Ok(ApiResponseFactory.Success( null,"Xóa thành công"));
            }
            catch (Exception ex)
            {

                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}