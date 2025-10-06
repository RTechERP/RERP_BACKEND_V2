using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeEarlyLate
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    /// <summary>
    /// Người duyệt (Từ bảng Employee)
    /// </summary>
    public int? ApprovedID { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? DateRegister { get; set; }

    public decimal? TimeRegister { get; set; }

    /// <summary>
    /// Đơn vị thời gian. 1:Giờ, 2: phút
    /// </summary>
    public int? Unit { get; set; }

    /// <summary>
    /// 1: Đi muộn việc cá nhân; 2: Về sớm việc cá nhân;  3: Về sớm việc công ty; 4:Đi muộn việc  công ty; 
    /// </summary>
    public int? Type { get; set; }

    public string? Reason { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ApprovedTP { get; set; }

    public bool? IsApprovedTP { get; set; }

    /// <summary>
    /// 2: Không đồng ý duyệt; 1: Có đồng ý duyệt
    /// </summary>
    public int? DecilineApprove { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public string? ReasonDeciline { get; set; }

    public string? ReasonHREdit { get; set; }

    public bool? IsProblem { get; set; }
}
