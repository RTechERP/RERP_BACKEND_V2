using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HistoryProductPriceRequest
{
    public int ID { get; set; }

    /// <summary>
    /// 1: Yêu cầu báo giá, 2: Yêu cầu mua
    /// </summary>
    public string? HistoryType { get; set; }

    public int? SupplierSaleID { get; set; }

    public int? CurrencyID { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductName { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? VAT { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? TotaMoneyVAT { get; set; }

    public decimal? TotalPriceExchange { get; set; }

    public int? TotalDayLeadTime { get; set; }

    public string? Note { get; set; }

    public decimal? HistoryPrice { get; set; }

    public decimal? CurrencyRate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
