using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PaymentOrderApproveFollow
{
    public int ID { get; set; }

    /// <summary>
    /// 1: mặc định; 2: Bỏ qua hr; 3: đề nghị đặc biệt
    /// </summary>
    public int? FollowType { get; set; }

    public string? Code { get; set; }

    public int? Step { get; set; }

    public string? StepName { get; set; }

    /// <summary>
    /// EmployeeID người duyệt mặc định
    /// </summary>
    public int? ApproverID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
