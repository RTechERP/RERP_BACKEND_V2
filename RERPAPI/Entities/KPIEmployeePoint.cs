using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPIEmployeePoint
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public int? KPIEvaluationRuleID { get; set; }

    public int? Status { get; set; }

    public decimal? TotalPercent { get; set; }

    public bool? IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsPublish { get; set; }

    public decimal? TotalPercentActual { get; set; }
}
