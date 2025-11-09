using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class PaymentOrderLog
{
    public int ID { get; set; }

    public int? PaymentOrderID { get; set; }

    public int? Step { get; set; }

    public string? StepName { get; set; }

    public DateTime? DateApproved { get; set; }

    /// <summary>
    /// 0:Chờ duyêt; 1:Đã duyệt; 2:Không duyệt
    /// </summary>
    public int? IsApproved { get; set; }

    /// <summary>
    /// Người được chỉ định duyệt
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Người duyệt thực tế
    /// </summary>
    public int? EmployeeApproveActualID { get; set; }

    public string? ReasonCancel { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? ContentLog { get; set; }

    /// <summary>
    /// Kế toán yc bổ sung (1: Yc bổ sung file)
    /// </summary>
    public bool? IsRequestAppendFileAC { get; set; }

    /// <summary>
    /// HR yc bổ sung (1: Yc bổ sung file)
    /// </summary>
    public bool? IsRequestAppendFileHR { get; set; }

    /// <summary>
    /// Lý do Kế toán yc bổ sung file
    /// </summary>
    public string? ReasonRequestAppendFileAC { get; set; }

    /// <summary>
    /// Lý do HR yc bổ sung file
    /// </summary>
    public string? ReasonRequestAppendFileHR { get; set; }
}
