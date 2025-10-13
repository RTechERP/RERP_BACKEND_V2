using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectPartlistPriceRequest
{
    public int ID { get; set; }

    public int? ProjectPartListID { get; set; }

    public int? EmployeeID { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductName { get; set; }

    /// <summary>
    /// 1:Yêu cầu báo giá; 2:Đã báo giá
    /// </summary>
    public int? StatusRequest { get; set; }

    public DateTime? DateRequest { get; set; }

    public DateTime? Deadline { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Unit { get; set; }

    public int? SupplierSaleID { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DatePriceQuote { get; set; }

    public decimal? TotalPriceExchange { get; set; }

    public decimal? CurrencyRate { get; set; }

    public int? CurrencyID { get; set; }

    public decimal? HistoryPrice { get; set; }

    public string? LeadTime { get; set; }

    public decimal? UnitFactoryExportPrice { get; set; }

    public decimal? UnitImportPrice { get; set; }

    public decimal? TotalImportPrice { get; set; }

    public bool? IsImport { get; set; }

    public bool? IsDeleted { get; set; }

    public int? QuoteEmployeeID { get; set; }

    public bool? IsCheckPrice { get; set; }

    public decimal? VAT { get; set; }

    public decimal? TotaMoneyVAT { get; set; }

    public int? TotalDayLeadTime { get; set; }

    public DateTime? DateExpected { get; set; }

    public int? POKHDetailID { get; set; }

    public bool? IsCommercialProduct { get; set; }

    public string? Maker { get; set; }

    public bool? IsJobRequirement { get; set; }
}
