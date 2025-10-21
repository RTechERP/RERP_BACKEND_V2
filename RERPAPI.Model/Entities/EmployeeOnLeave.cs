using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeOnLeave
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public int? ApprovedTP { get; set; }

    public int? ApprovedHR { get; set; }

    /// <summary>
    /// 1: Nghỉ buổi sáng; 2:Nghỉ buổi chiều; 3:Nghỉ cả ngày
    /// </summary>
    public int? TimeOnLeave { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? TotalTime { get; set; }

    /// <summary>
    /// 1: Nghỉ không lương; 2: Nghỉ phép
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// 1: Nghỉ không lương; 2: Nghỉ phép
    /// </summary>
    public int? TypeIsReal { get; set; }

    public decimal? TotalDay { get; set; }

    public string? Reason { get; set; }

    public string? Note { get; set; }

    public bool? IsApprovedTP { get; set; }

    public bool? IsApprovedHR { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsCancelTP { get; set; }

    public bool? IsCancelHR { get; set; }

    public bool? IsCancelRegister { get; set; }

    /// <summary>
    /// 2: Không đồng ý duyệt; 1: Có đồng ý duyệt
    /// </summary>
    public int? DecilineApprove { get; set; }

    public string? ReasonCancel { get; set; }

    public DateTime? DateCancel { get; set; }

    public bool? DeleteFlag { get; set; }

    public string? ReasonDeciline { get; set; }

    public string? ReasonHREdit { get; set; }

    public bool? IsProblem { get; set; }

    public bool? IsApprovedBGD { get; set; }

    public int? ApprovedBGDID { get; set; }

    public DateTime? DateApprovedBGD { get; set; }
}
