using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleRankingResult
{
    public int Id { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? EmployeeName { get; set; }

    public int? PeriodId { get; set; }

    public int? TemplateId { get; set; }

    public string? TeamCode { get; set; }

    public string? PositionType { get; set; }

    public decimal? TotalSalesAmount { get; set; }

    public decimal? AchievementPercent { get; set; }

    public decimal? Coefficient { get; set; }

    public int? Ranking { get; set; }

    public decimal? SalesBonusAmount { get; set; }

    public decimal? RankingBonusAmount { get; set; }

    public int? NewAccountCount { get; set; }

    public decimal? NewAccountBonus { get; set; }

    public decimal? OtherBonus { get; set; }

    public decimal? TotalBonus { get; set; }

    public int? RewardConfigId { get; set; }

    public bool? IsCalculated { get; set; }

    public DateTime? CalculatedDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public decimal TotalRevenue { get; set; }
}
