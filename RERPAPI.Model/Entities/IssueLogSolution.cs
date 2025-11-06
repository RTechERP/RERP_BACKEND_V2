using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class IssueLogSolution
{
    public int ID { get; set; }

    public DateTime? DateIssue { get; set; }

    public int? DepartmentID { get; set; }

    public int? RelatedDepartmentID { get; set; }

    public string? IssueDescription { get; set; }

    public int? CustomerID { get; set; }

    public int? SupplierID { get; set; }

    public int? ProjectID { get; set; }

    public string? ImpactDetail { get; set; }

    public string? ImmediateAction { get; set; }

    public string? PreventiveAction { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? Deadline { get; set; }

    public int? VerifiedBy { get; set; }

    public string? Note { get; set; }

    public int? IssueSolutionType { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
