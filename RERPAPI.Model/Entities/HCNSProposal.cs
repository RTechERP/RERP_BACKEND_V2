using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HCNSProposal
{
    public int ID { get; set; }

    public int? DepartmentRequiredID { get; set; }

    public string? ProductName { get; set; }

    public string? Supplier { get; set; }

    public string? Contact { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? STT { get; set; }

    public int? JobRequirementID { get; set; }

    public int? IsApproved { get; set; }

    public string? DisapprovalReason { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public int? ApproverID { get; set; }
}
