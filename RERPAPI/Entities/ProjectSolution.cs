using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectSolution
{
    public int ID { get; set; }

    public int? ProjectRequestID { get; set; }

    public int? STT { get; set; }

    public DateTime? DateSolution { get; set; }

    public string? CodeSolution { get; set; }

    public string? ContentSolution { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? StatusSolution { get; set; }

    public bool? IsApprovedPrice { get; set; }

    public bool? IsApprovedPO { get; set; }

    public int? EmployeeApprovedPriceID { get; set; }

    public int? EmployeeApprovedPOID { get; set; }

    public DateTime? PriceReportDeadline { get; set; }

    public bool? IsDeleted { get; set; }
}
