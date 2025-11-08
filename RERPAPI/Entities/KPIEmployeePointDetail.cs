using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPIEmployeePointDetail
{
    public int ID { get; set; }

    public int? KPIEmployeePointID { get; set; }

    public int? KPIEvaluationRuleDetailID { get; set; }

    public decimal? FirstMonth { get; set; }

    public decimal? SecondMonth { get; set; }

    public decimal? ThirdMonth { get; set; }

    public decimal? PercentBonus { get; set; }

    public decimal? PercentRemaining { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
