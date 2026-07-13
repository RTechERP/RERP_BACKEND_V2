using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleApproval
{
    public int ID { get; set; }

    public string ApprovalScope { get; set; } = null!;

    public int? EmployeeID { get; set; }

    public int? TeamID { get; set; }

    public int PeriodID { get; set; }

    public string CurrentStep { get; set; } = null!;

    public bool IsAdminApproved { get; set; }

    public string? AdminApprovedBy { get; set; }

    public DateTime? AdminApprovedDate { get; set; }

    public bool IsSalesManagerApproved { get; set; }

    public string? SalesManagerApprovedBy { get; set; }

    public DateTime? SalesManagerApprovedDate { get; set; }

    public bool IsAccountantApproved { get; set; }

    public string? AccountantApprovedBy { get; set; }

    public DateTime? AccountantApprovedDate { get; set; }

    public bool IsSeniorAccountantApproved { get; set; }

    public string? SeniorAccountantApprovedBy { get; set; }

    public DateTime? SeniorAccountantApprovedDate { get; set; }

    public bool IsDirectorApproved { get; set; }

    public string? DirectorApprovedBy { get; set; }

    public DateTime? DirectorApprovedDate { get; set; }

    public bool IsHRDisbursed { get; set; }

    public string? HRDisbursedBy { get; set; }

    public DateTime? HRDisbursedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
