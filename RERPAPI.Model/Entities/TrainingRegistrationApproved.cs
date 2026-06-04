using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TrainingRegistrationApproved
{
    public int ID { get; set; }

    public int? TrainingRegistrationID { get; set; }

    public int? TrainingRegistrationApprovedFlowID { get; set; }

    /// <summary>
    /// Người duyệt mặc định
    /// </summary>
    public int? EmployeeApprovedID { get; set; }

    /// <summary>
    /// Người duyệt thực tế
    /// </summary>
    public int? EmployeeApprovedActualID { get; set; }

    public DateTime? DateApproved { get; set; }

    /// <summary>
    /// Trạng thái: 1: Đã duyệt; 2 Hủy duyệt...
    /// </summary>
    public int? StatusApproved { get; set; }

    public string? Note { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UnapprovedReason { get; set; }
}
