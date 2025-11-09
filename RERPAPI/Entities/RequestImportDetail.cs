using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RequestImportDetail
{
    public int ID { get; set; }

    public string? ProductCode { get; set; }

    public decimal Qty { get; set; }

    public string? Maker { get; set; }

    public string? ProductName { get; set; }

    public string Unit { get; set; } = null!;

    public string? WareHouse { get; set; }

    public string? Project { get; set; }

    public string? POSuplier { get; set; }

    public string? Note { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? IntoMoney { get; set; }

    public string? Pay { get; set; }

    public string? Suplier { get; set; }

    public string? RequestCode { get; set; }

    public int? RequestImportID { get; set; }
}
