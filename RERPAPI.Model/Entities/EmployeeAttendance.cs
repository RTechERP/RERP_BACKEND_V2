using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeAttendance
{
    public int ID { get; set; }

    public string IDChamCongMoi { get; set; } = null!;

    public int? EmployeeID { get; set; }

    public string? CheckIn { get; set; }

    public string? CheckOut { get; set; }

    public bool? IsLate { get; set; }

    public decimal? TimeLate { get; set; }

    public bool? IsEarly { get; set; }

    public decimal? TimeEarly { get; set; }

    public DateTime? AttendanceDate { get; set; }

    public string? DayWeek { get; set; }

    /// <summary>
    /// Khoảng thời gian trong ngày(0h - 24h)
    /// </summary>
    public string? Interval { get; set; }

    public int? STT { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public decimal? TotalHour { get; set; }

    public bool? IsLunch { get; set; }

    public decimal? TotalDay { get; set; }

    public string? Note { get; set; }

    public DateTime? CheckInDate { get; set; }

    public DateTime? CheckOutDate { get; set; }
}
