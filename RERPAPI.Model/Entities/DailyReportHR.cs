using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class DailyReportHR
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? DateReport { get; set; }

    public int? FilmManagementDetailID { get; set; }

    /// <summary>
    /// Kết quả thực hiện cắt phim
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// Thời gian thực hiện
    /// </summary>
    public decimal TimeActual { get; set; }

    /// <summary>
    /// TimeActual / Quantity (năng suất thực tế)
    /// </summary>
    public decimal? PerformanceActual { get; set; }

    /// <summary>
    /// tỷ lệ năng suất trung bình / năng xuất thực tế
    /// </summary>
    public decimal? Percentage { get; set; }

    /// <summary>
    /// Số km
    /// </summary>
    public decimal? KmNumber { get; set; }

    /// <summary>
    /// Số cuốc xe muộn so với lịch đặt xe
    /// </summary>
    public int? TotalLate { get; set; }

    /// <summary>
    /// Tống số phút chậm
    /// </summary>
    public decimal? TotalTimeLate { get; set; }

    public string? ReasonLate { get; set; }

    public string? StatusVehicle { get; set; }

    /// <summary>
    /// Kiến nghị / đề xuất
    /// </summary>
    public string? Propose { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
