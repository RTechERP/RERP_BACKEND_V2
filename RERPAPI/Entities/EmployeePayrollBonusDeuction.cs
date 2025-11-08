using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class EmployeePayrollBonusDeuction
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public int? YearValue { get; set; }

    public int? MonthValue { get; set; }

    /// <summary>
    /// Thưởng  KPIs / doanh số
    /// </summary>
    public decimal? KPIBonus { get; set; }

    /// <summary>
    /// Thưởng khác
    /// </summary>
    public decimal? OtherBonus { get; set; }

    /// <summary>
    /// Gửi xe ô tô
    /// </summary>
    public decimal? ParkingMoney { get; set; }

    /// <summary>
    /// Phạt 5s
    /// </summary>
    public decimal? Punish5S { get; set; }

    /// <summary>
    /// Khoản trừ khác
    /// </summary>
    public decimal? OtherDeduction { get; set; }

    /// <summary>
    /// Mức đóng bảo hiểm xã hội
    /// </summary>
    public decimal? BHXH { get; set; }

    /// <summary>
    /// Tạm ứng lương
    /// </summary>
    public decimal? SalaryAdvance { get; set; }

    /// <summary>
    /// Mức thu BHXH
    /// </summary>
    public decimal? Insurances { get; set; }

    /// <summary>
    /// Tổng ngày công được hưởng
    /// </summary>
    public decimal? TotalWorkDay { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
