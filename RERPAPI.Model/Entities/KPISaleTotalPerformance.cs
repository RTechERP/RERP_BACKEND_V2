using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleTotalPerformance
{
    public long ID { get; set; }

    public int? EmployeeID { get; set; }

    public int? PeriodID { get; set; }

    public int? TemplateID { get; set; }

    public decimal? FinalScore { get; set; }

    public DateTime? CalculatedDate { get; set; }

    public int? TeamID { get; set; }
}
