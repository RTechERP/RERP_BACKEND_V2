using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class QuotationKHDetail
{
    public int ID { get; set; }

    public string? InternalCode { get; set; }

    public string? InternalName { get; set; }

    public decimal? Qty { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? IntoMoney { get; set; }

    public string? Image { get; set; }

    public string? Note { get; set; }

    public int? QuotationKHID { get; set; }

    public int? ProductID { get; set; }

    public int? STT { get; set; }

    public decimal? GiaNet { get; set; }

    public string? ProductName { get; set; }

    public string? Maker { get; set; }

    public string? Unit { get; set; }

    public decimal? UnitPriceImport { get; set; }

    public decimal? TotalPriceImport { get; set; }

    public string? ProductCode { get; set; }

    public string? GroupQuota { get; set; }

    public int? IsPO { get; set; }

    public int? POKHID { get; set; }

    public bool? IsSelected { get; set; }

    public string? ProductNewCode { get; set; }

    public string? TypeOfPrice { get; set; }

    public bool IsDeleted { get; set; }
}
