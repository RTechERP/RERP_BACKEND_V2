using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPIEvaluationRuleDetail
{
    public int ID { get; set; }

    public string? STT { get; set; }

    public int? KPIEvaluationID { get; set; }

    public int? KPIEvaluationRuleID { get; set; }

    public int? ParentID { get; set; }

    public string? RuleContent { get; set; }

    public string? FormulaCode { get; set; }

    public decimal? MaxPercent { get; set; }

    public decimal? PercentageAdjustment { get; set; }

    public decimal? MaxPercentageAdjustment { get; set; }

    public string? RuleNote { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
