using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleRewardCoefficient
{
    public int Id { get; set; }

    public int? ConfigId { get; set; }

    public string? EmployeeType { get; set; }

    public decimal? MinPerformance { get; set; }

    public decimal? MaxPerformance { get; set; }

    public decimal? Coefficient { get; set; }

    public int? Priority { get; set; }

    public bool? IsActive { get; set; }
}
