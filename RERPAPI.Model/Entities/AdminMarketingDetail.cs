using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AdminMarketingDetail
{
    public int ID { get; set; }

    public int? Month1 { get; set; }

    public int? Month2 { get; set; }

    public int? Month3 { get; set; }

    public int? Quy { get; set; }

    public int? UserID { get; set; }

    public int? QuantityActual { get; set; }

    public decimal? PercentActual { get; set; }

    public int? KPIID { get; set; }

    public decimal? CompletionRate { get; set; }

    public int? Quantity { get; set; }

    public int? Year { get; set; }
}
