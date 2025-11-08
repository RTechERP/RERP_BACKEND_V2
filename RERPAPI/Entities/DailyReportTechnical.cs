using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class DailyReportTechnical
{
    public int ID { get; set; }

    public int? MasterID { get; set; }

    public int? UserReport { get; set; }

    public DateOnly? DateReport { get; set; }

    public int? ProjectID { get; set; }

    public string? Content { get; set; }

    public string? Results { get; set; }

    public string? Problem { get; set; }

    public string? ProblemSolve { get; set; }

    public string? PlanNextDay { get; set; }

    public string? Note { get; set; }

    public bool? Confirm { get; set; }

    public string? Backlog { get; set; }

    public int? DeleteFlag { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? Type { get; set; }

    public int? ReportLate { get; set; }

    public int? OldProjectID { get; set; }

    public decimal? TotalHours { get; set; }

    public int? StatusResult { get; set; }

    public int? WorkPlanDetailID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ProjectItemID { get; set; }

    public int? PercentComplete { get; set; }

    public decimal? TotalHourOT { get; set; }
}
