using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeSalaryAdvance
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public decimal? Money { get; set; }

    public DateTime? DateRequest { get; set; }

    /// <summary>
    /// trưởng phòng or người quản lý trực tiếp đồng ý
    /// </summary>
    public bool? IsApproved_TP { get; set; }

    /// <summary>
    /// Trường phòng nhân sự đồng ý
    /// </summary>
    public bool? IsApproved_HR { get; set; }

    /// <summary>
    /// Trưởng phòng kế toán đồng ý
    /// </summary>
    public bool? IsApproved_KT { get; set; }

    /// <summary>
    /// Trưởng phòng Kỹ thuật
    /// </summary>
    public int? ApprovedTP { get; set; }

    /// <summary>
    /// Trưởng phòng nhân sự
    /// </summary>
    public int? ApprovedHR { get; set; }

    /// <summary>
    /// Trưởng phòng kế toán
    /// </summary>
    public int? ApprovedKT { get; set; }

    public bool? IsPayed { get; set; }

    public string? Reason { get; set; }

    public DateTime? DatePayed { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// 2: Không đồng ý duyệt; 1: Có đồng ý duyệt
    /// </summary>
    public int? DecilineApprove { get; set; }

    public string? ReasonDeciline { get; set; }
}
