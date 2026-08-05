using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleRewardConfig
{
    public int Id { get; set; }

    public string? ConfigCode { get; set; }

    public string? ConfigName { get; set; }

    public string? EmployeeType { get; set; }

    public decimal? RewardRate { get; set; }

    public decimal? Rank1BonusAmount { get; set; }

    public decimal? NewAccountBonusAmount { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? TemplateId { get; set; }

    public int? NewAccountKpiIndexId { get; set; }

    public int? SalesAmountKpiIndexId { get; set; }

    public int? RevenueKpiIndexId { get; set; }
}
