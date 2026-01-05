using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProposalApproval
{
    public int ID { get; set; }

    public int? DepartmentRequiredID { get; set; }

    public int? HCNSProposalsID { get; set; }

    public int? ApproverID { get; set; }

    public int? IsApproved { get; set; }

    public string? DisapprovalReason { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
