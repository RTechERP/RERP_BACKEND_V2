using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeFingerprint
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? IDChamCong { get; set; }

    public int? IDFingerprintMaster { get; set; }

    public int? UserID { get; set; }

    public string? Organization { get; set; }

    public DateTime? Day { get; set; }

    public string? DayOfWeek { get; set; }

    public string? Period { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public string? Note { get; set; }

    /// <summary>
    /// Lấy giá trị tính theo giờ
    /// </summary>
    public decimal? TotalTime { get; set; }

    public decimal? WorkTime { get; set; }

    public bool? Diligent { get; set; }

    public bool? IsLate { get; set; }

    public bool? IsEarly { get; set; }

    /// <summary>
    /// Lấy giá trị tính theo phút
    /// </summary>
    public decimal? SumLate { get; set; }

    /// <summary>
    /// lấy giá trị tính theo phút
    /// </summary>
    public decimal? SumEarly { get; set; }

    public int? IsLunch { get; set; }

    public decimal? TimeReal { get; set; }
}
