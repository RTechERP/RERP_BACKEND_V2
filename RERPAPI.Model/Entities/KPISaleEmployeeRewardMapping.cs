using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleEmployeeRewardMapping
{
    public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public int? RewardConfigId { get; set; }

    public string? PositionType { get; set; }

    public string? TeamCode { get; set; }

    public string? ProjectIds { get; set; }

    public bool? IsActive { get; set; }

    public DateOnly? EffectiveFromDate { get; set; }

    public DateOnly? EffectiveToDate { get; set; }
}
