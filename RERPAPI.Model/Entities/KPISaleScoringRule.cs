using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleScoringRule
{
    public int ID { get; set; }

    public int KpiIndexID { get; set; }

    public string ScoreType { get; set; } = null!;

    public decimal? MaxAchievedPercent { get; set; }

    public string? FormulaJson { get; set; }
}
