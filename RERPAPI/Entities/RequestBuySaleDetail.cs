using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RequestBuySaleDetail
{
    public int ID { get; set; }

    public int? ProductID { get; set; }

    public string? ContactName { get; set; }

    public int? ContactPhone { get; set; }

    public decimal? Qty { get; set; }

    public decimal? QtyBuy { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? IntoMoney { get; set; }

    public DateTime? DeadLine { get; set; }

    public string? TargetUse { get; set; }

    public string? Note { get; set; }

    public int? SuplierID { get; set; }

    public int? RequestBuySaleID { get; set; }
}
