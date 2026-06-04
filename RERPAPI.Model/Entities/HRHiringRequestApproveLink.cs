using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HRHiringRequestApproveLink
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int HRHiringRequestID { get; set; }

    public int ApproveID { get; set; }

    /// <summary>
    /// 0: chờ duyệt; 1: Đã duyệt; 2: Hủy duyệt
    /// </summary>
    public int IsApprove { get; set; }

    public DateTime? DateApprove { get; set; }

    public int Step { get; set; }

    public string? StepName { get; set; }

    public string? ReasonUnApprove { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }
}
