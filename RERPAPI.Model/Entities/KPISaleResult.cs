using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleResult
{
    public long ID { get; set; }

    public int EmployeeID { get; set; }

    public int PeriodID { get; set; }

    public int KpiIndexID { get; set; }

    public decimal GoalValue { get; set; }

    public decimal ResultValue { get; set; }

    public decimal AchievedPercent { get; set; }

    public decimal WeightPercent { get; set; }

    public decimal FinalScore { get; set; }

    public DateTime CalculatedDate { get; set; }

    public int? ReportScoreAdjustmentType { get; set; }

    public decimal? ReportScoreValue { get; set; }

    public int? TeamID { get; set; }
}
