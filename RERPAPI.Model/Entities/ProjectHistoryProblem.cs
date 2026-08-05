using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectHistoryProblem
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? STT { get; set; }

    public string? TypeProblem { get; set; }

    public string? ContentError { get; set; }

    public string? Reason { get; set; }

    public string? Remedies { get; set; }

    public string? TestMethod { get; set; }

    public string? Image { get; set; }

    public DateTime? DateProblem { get; set; }

    public DateTime? DateImplementation { get; set; }

    public string? PIC { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? EmployeeID { get; set; }

    public bool? IsDeleted { get; set; }

    public int? IssueLogType { get; set; }

    public int? CreatorID { get; set; }

    public int? PriorityLevel { get; set; }

    public int? PerformerID { get; set; }

    public int? StatusProblem { get; set; }

    public string? IssueConclusion { get; set; }

    public bool? IsApproved_PM { get; set; }

    public DateTime? DateApproved_PM { get; set; }

    public bool? IsApproved_TP { get; set; }

    public DateTime? DateApproved_TP { get; set; }

    public bool? IsApproved_PP { get; set; }

    public DateTime? DateApproved_PP { get; set; }

    public string? Note { get; set; }

    public string? ErrorLocation { get; set; }

    public string? Impact { get; set; }

    public int? ProjectManagerID { get; set; }
}
