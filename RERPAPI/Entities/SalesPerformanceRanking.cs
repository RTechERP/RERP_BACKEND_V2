using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class SalesPerformanceRanking
{
    public int ID { get; set; }

    public decimal? Performance { get; set; }

    public decimal? Coefficient { get; set; }

    public int? UserID { get; set; }

    public int? Quy { get; set; }

    public int? Year { get; set; }

    public decimal? PerformanceOld { get; set; }

    public decimal? TotalSale { get; set; }

    public decimal? BonusRank { get; set; }

    public decimal? BonusAdd { get; set; }

    public decimal? BonusSales { get; set; }

    public decimal? BonusAcc { get; set; }

    public decimal? TotalBonus { get; set; }

    public int? NewAccountQty { get; set; }

    public int? WarehouseID { get; set; }
}
