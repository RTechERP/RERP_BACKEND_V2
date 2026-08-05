using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleTarget
{
    public int ID { get; set; }

    public int EmployeeID { get; set; }

    public int PeriodID { get; set; }

    public int KpiIndexID { get; set; }

    public decimal GoalValue { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? TeamID { get; set; }

    public decimal? WeightPercent { get; set; }

    public decimal? ProposedGoalValue { get; set; }

    public decimal? ProposedWeightPercent { get; set; }

    public string? ApprovalStatus { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public string? RejectedBy { get; set; }

    public DateTime? RejectedDate { get; set; }

    public bool? IsBoardApproved { get; set; }

    public string? BoardApprovedBy { get; set; }

    public DateTime? BoardApprovedDate { get; set; }
}
