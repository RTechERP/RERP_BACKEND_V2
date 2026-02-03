using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeWFH
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public int? ApprovedID { get; set; }

    public bool? IsApproved { get; set; }

    public string? Reason { get; set; }

    public DateTime? DateWFH { get; set; }

    /// <summary>
    /// 1: Buổi sáng; 2:Buổi chiều, 3: Cả ngày
    /// </summary>
    public int? TimeWFH { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public decimal? TotalDay { get; set; }

    public int? ApprovedHR { get; set; }

    public bool? IsApprovedHR { get; set; }

    /// <summary>
    /// 2: Không đồng ý duyệt; 1: Có đồng ý duyệt
    /// </summary>
    public int? DecilineApprove { get; set; }

    public string? ReasonDeciline { get; set; }

    public string? ReasonHREdit { get; set; }

    public bool? IsProblem { get; set; }

    public string? ContentWork { get; set; }

    public bool? IsApprovedBGD { get; set; }

    public int? ApprovedBGDID { get; set; }

    public DateTime? DateApprovedBGD { get; set; }

    public string? EvaluateResults { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Trạng thái Senior duyệt (0: Chờ duyệt | 1: Đã duyệt | 2: Không duyệt)
    /// </summary>
    public int? IsSeniorApproved { get; set; }

    /// <summary>
    /// ID Senior duyệt
    /// </summary>
    public int? ApprovedSeniorID { get; set; }

    /// <summary>
    /// Ngày Senior  duyệt
    /// </summary>
    public DateTime? DateApprovedSenior { get; set; }
}
