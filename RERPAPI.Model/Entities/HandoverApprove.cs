using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HandoverApprove
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? HandoverID { get; set; }

    public int? EmployeeID { get; set; }

    public int? ApproveStatus { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Note { get; set; }

    public int? ApproveLevel { get; set; }

    public string? RoleName { get; set; }

    public DateTime? ApproveDate { get; set; }

    public int? ApproverID { get; set; }

    public string? RejectReason { get; set; }
}
