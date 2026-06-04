using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectPartlistPriceRequestHistory
{
    public int ID { get; set; }

    public int? ProjectPartlistPriceRequestID { get; set; }

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

    public int? QuoteEmployeeID { get; set; }

    public bool? IsSelectedQuote { get; set; }

    public bool? IsDeleted { get; set; }
}
