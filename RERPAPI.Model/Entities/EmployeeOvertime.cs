using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeOvertime
{
    public int ID { get; set; }

    public bool? IsApproved { get; set; }

    public int? EmployeeID { get; set; }

    public int? ApprovedID { get; set; }

    public DateTime? DateRegister { get; set; }

    /// <summary>
    /// 1:Văn phòng; 2;Địa điểm công tác
    /// </summary>
    public int? Location { get; set; }

    public int? TypeID { get; set; }

    public DateTime? TimeStart { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal? TimeReality { get; set; }

    /// <summary>
    ///  = Ratio * TimeReality
    /// </summary>
    public decimal? TotalTime { get; set; }

    public decimal? CostOvertime { get; set; }

    public string? Note { get; set; }

    public bool? Overnight { get; set; }

    public decimal? CostOvernight { get; set; }

    public string? Reason { get; set; }

    /// <summary>
    /// 2: Không đồng ý duyệt; 1: Có đồng ý duyệt
    /// </summary>
    public int? DecilineApprove { get; set; }

    public int? ApprovedHR { get; set; }

    public bool? IsApprovedHR { get; set; }

    public string? ReasonDeciline { get; set; }

    public int? ProjectID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? ReasonHREdit { get; set; }

    public bool? IsProblem { get; set; }

    public bool? IsApprovedBGD { get; set; }

    public int? ApprovedBGDID { get; set; }

    public DateTime? DateApprovedBGD { get; set; }

    public bool? IsSeniorApproved { get; set; }

    public int? ApprovedSeniorID { get; set; }

    public DateTime? DateApprovedSenitor { get; set; }
}
