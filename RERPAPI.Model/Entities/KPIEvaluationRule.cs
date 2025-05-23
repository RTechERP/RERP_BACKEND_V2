using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPIEvaluationRule
{
    public int ID { get; set; }

    public int? KPISessionID { get; set; }

    public int? KPIPositionID { get; set; }

    public string? RuleCode { get; set; }

    public string? RuleName { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
