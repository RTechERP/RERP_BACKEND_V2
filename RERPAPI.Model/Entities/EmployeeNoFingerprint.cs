using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeNoFingerprint
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public int? ApprovedTP { get; set; }

    public DateTime? DayWork { get; set; }

    public bool? IsApprovedTP { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? ApprovedHR { get; set; }

    public bool? IsApprovedHR { get; set; }

    /// <summary>
    /// 1:Quên buổi sáng; 2:Quên buổi chiều;3:Quên chấm công do đi công tác
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// 2: Không đồng ý duyệt; 1: Có đồng ý duyệt
    /// </summary>
    public int? DecilineApprove { get; set; }

    public string? ReasonDeciline { get; set; }

    public string? ReasonHREdit { get; set; }

    public bool? IsDeleted { get; set; }
}
