using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class EmployeeChamCongDetail
{
    public int ID { get; set; }

    public int? MasterID { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? DayFinger { get; set; }

    public decimal? WorkTime { get; set; }

    public int? NoFinger { get; set; }

    public int? TypeBussiness { get; set; }

    public decimal? CostBussiness { get; set; }

    public int? TypeOvertime { get; set; }

    public decimal? TotalTimeOT { get; set; }

    public int? Overnight { get; set; }

    public int? OnLeaveType { get; set; }

    public decimal? OnLeaveDay { get; set; }

    public decimal? CostWorkEarly { get; set; }

    public decimal? TotalDayWFH { get; set; }

    public string? TotalDayText { get; set; }

    public decimal? TotalDay { get; set; }

    public string? TotalLunchText { get; set; }

    public int? TotalLunch { get; set; }

    public int? IsEarly { get; set; }

    public int? IsLate { get; set; }

    public decimal? TotalNightShift { get; set; }

    /// <summary>
    /// Ngày công theo bảng chấm vân tay
    /// </summary>
    public decimal? TotalDayFinger { get; set; }

    /// <summary>
    /// Không chấm công tại Vp khi đi công tác
    /// </summary>
    public bool? NotCheckin { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? FoodOrderText { get; set; }

    public int? FoodOrderUse { get; set; }

    public string? IDChamCong { get; set; }

    public bool? IsDeleted { get; set; }
}
