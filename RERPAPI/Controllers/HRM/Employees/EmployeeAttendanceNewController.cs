
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using RERPAPI.Attributes;
using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using RERPAPI.Repo.GenericEntity;
using System;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RERPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAttendanceNewController : ControllerBase
    {
        private readonly EmployeeRepo _employeeRepo;
        private readonly EmployeeAttendanceRepo _employeeAttendanceRepo;
        private readonly DepartmentRepo _departmentRepo;
        private readonly EmployeeAttendanceNewRepo _employeeAttendanceNewRepo;
        public EmployeeAttendanceNewController(EmployeeRepo employeeRepo, EmployeeAttendanceRepo employeeAttendanceRepo, DepartmentRepo departmentRepo, EmployeeAttendanceNewRepo employeeAttendanceNewRepo)
        {
            _employeeRepo = employeeRepo;
            _employeeAttendanceRepo = employeeAttendanceRepo;
            _departmentRepo = departmentRepo;
            _employeeAttendanceNewRepo = employeeAttendanceNewRepo;
        }

        //[HttpPost("save-attendance")]
        //public async Task<IActionResult> saveAttendance([FromBody] List<EmployeeAttendanceNewDTO> data)
        //{
        //    try
        //    {
        //        if (data == null || data.Count == 0)
        //            return Ok(ApiResponseFactory.Success(null, "Không có dữ liệu"));

        //        var groups = data
        //            .Where(x => !string.IsNullOrWhiteSpace(x.employeeNoString))
        //            .GroupBy(x => x.employeeNoString.Trim().ToLower());

        //        foreach (var group in groups)
        //        {
        //            string employeeCode = group.Key;

        //            var emp = _employeeRepo
        //                .GetAll(x => x.IDChamCongMoi.ToLower() == employeeCode)
        //                .FirstOrDefault();

        //            if (emp == null) continue;

        //            DateTime checkInTime = group.Min(x => x.time);
        //            DateTime checkOutTime = group.Max(x => x.time);

        //            DateTime attendanceDate = checkInTime.Date;
        //            string dayOfWeek = GetVietnameseDayOfWeek(checkInTime.DayOfWeek);
        //            var comp = ComputeAttendance(0, attendanceDate, checkInTime, checkOutTime);

        //            var checkExist = _employeeAttendanceNewRepo.GetAll(x => x.EmployeeID == emp.ID && x.AttendanceDate == attendanceDate).FirstOrDefault();
        //            if (checkExist != null)
        //            {
        //                checkExist.CheckOutDate = checkOutTime;
        //                checkExist.CheckOut = checkOutTime.ToString("HH:mm:ss");
        //                checkExist.UpdatedDate = DateTime.Now;
        //                checkExist.IsLate = comp.IsLate;
        //                checkExist.TimeLate = comp.TimeLate;
        //                checkExist.IsEarly = comp.IsEarly;
        //                checkExist.TimeEarly = comp.TimeEarly;
        //                checkExist.TotalHour = comp.TotalHour;
        //                checkExist.TotalDay = comp.TotalDay;
        //                checkExist.IsLunch = comp.IsLunch;
        //                await _employeeAttendanceNewRepo.UpdateAsync(checkExist);
        //                continue;
        //            }

        //            EmployeeAttendanceNew attendance = new EmployeeAttendanceNew
        //            {
        //                STT = 0,
        //                EmployeeID = emp.ID,
        //                IDChamCongMoi = emp.IDChamCongMoi,

        //                AttendanceDate = attendanceDate,
        //                DayWeek = dayOfWeek,

        //                Interval = "(00:00:00-23:59:00)",

        //                // giờ vào - giờ ra dạng string
        //                CheckIn = checkInTime.ToString("HH:mm:ss"),
        //                CheckOut = checkOutTime.ToString("HH:mm:ss"),

        //                // datetime đầy đủ
        //                CheckInDate = checkInTime,
        //                CheckOutDate = checkOutTime,

        //                CreatedDate = DateTime.Now,
        //                UpdatedDate = DateTime.Now,
        //                IsLate = comp.IsLate,
        //                TimeLate = comp.TimeLate,
        //                IsEarly = comp.IsEarly,
        //                TimeEarly = comp.TimeEarly,
        //                TotalHour = comp.TotalHour,
        //                TotalDay = comp.TotalDay,
        //                IsLunch = comp.IsLunch,
        //            };

        //            await _employeeAttendanceNewRepo.CreateAsync(attendance);
        //        }

        //        return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}


        [HttpPost("save-attendance")]
        public async Task<IActionResult> saveAttendance([FromBody] List<EmployeeAttendanceNewDTO> data)
        {
            try
            {
                if (data == null || data.Count == 0)
                    return Ok(ApiResponseFactory.Success(null, "Không có dữ liệu"));

                var groups = data
                    .Where(x => !string.IsNullOrWhiteSpace(x.employeeNoString))
                    .GroupBy(x => x.employeeNoString.Trim().ToLower());

                foreach (var group in groups)
                {
                    string employeeCode = group.Key;

                    var emp = _employeeRepo
                        .GetAll(x => x.IDChamCongMoi.ToLower() == employeeCode)
                        .FirstOrDefault();

                    if (emp == null) continue;

                    DateTime checkInTime = group.Min(x => x.time);
                    DateTime checkOutTime = group.Max(x => x.time);

                    DateTime attendanceDate = checkInTime.Date;
                    string dayOfWeek = GetVietnameseDayOfWeek(checkInTime.DayOfWeek);
                    var comp = ComputeAttendance(0, attendanceDate, checkInTime, checkOutTime);

                    var checkExist = _employeeAttendanceRepo.GetAll(x => x.EmployeeID == emp.ID && x.AttendanceDate == attendanceDate).FirstOrDefault();
                    if (checkExist != null)
                    {
                        checkExist.CheckOutDate = checkOutTime;
                        checkExist.CheckOut = checkOutTime.ToString("HH:mm:ss");
                        checkExist.UpdatedDate = DateTime.Now;
                        checkExist.IsLate = comp.IsLate;
                        checkExist.TimeLate = comp.TimeLate;
                        checkExist.IsEarly = comp.IsEarly;
                        checkExist.TimeEarly = comp.TimeEarly;
                        checkExist.TotalHour = comp.TotalHour;
                        checkExist.TotalDay = comp.TotalDay;
                        checkExist.IsLunch = comp.IsLunch;
                        await _employeeAttendanceRepo.UpdateAsync(checkExist);
                        continue;
                    }

                    EmployeeAttendance attendance = new EmployeeAttendance
                    {
                        STT = 0,
                        EmployeeID = emp.ID,
                        IDChamCongMoi = emp.IDChamCongMoi,

                        AttendanceDate = attendanceDate,
                        DayWeek = dayOfWeek,

                        Interval = "(00:00:00-23:59:00)",

                        // giờ vào - giờ ra dạng string
                        CheckIn = checkInTime.ToString("HH:mm:ss"),
                        CheckOut = checkOutTime.ToString("HH:mm:ss"),

                        // datetime đầy đủ
                        CheckInDate = checkInTime,
                        CheckOutDate = checkOutTime,

                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsLate = comp.IsLate,
                        TimeLate = comp.TimeLate,
                        IsEarly = comp.IsEarly,
                        TimeEarly = comp.TimeEarly,
                        TotalHour = comp.TotalHour,
                        TotalDay = comp.TotalDay,
                        IsLunch = comp.IsLunch,
                    };

                    await _employeeAttendanceRepo.CreateAsync(attendance);
                }

                return Ok(ApiResponseFactory.Success(null, "Lưu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        private string GetVietnameseDayOfWeek(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Hai",
                DayOfWeek.Tuesday => "Ba",
                DayOfWeek.Wednesday => "Tư",
                DayOfWeek.Thursday => "Năm",
                DayOfWeek.Friday => "Sáu",
                DayOfWeek.Saturday => "Bảy",
                DayOfWeek.Sunday => "CN",
                _ => ""
            };
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
    }
}