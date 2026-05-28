using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class BillExportAcountantDetail
{
    public int ID { get; set; }

    public int? BillExportID { get; set; }

    public int? ProductID { get; set; }

    public int? BillExportSaleID { get; set; }

    public string? ProductFullName { get; set; }

    public decimal? Qty { get; set; }

    public string? ProjectName { get; set; }

    public int? ExportID { get; set; }

    public string? Note { get; set; }

    public int? STT { get; set; }

    public bool? IsInvoice { get; set; }

    public string? InvoiceNumber { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? VAT { get; set; }

    public decimal? IntoMoney { get; set; }

    public decimal? IntoMoneyWithoutVat { get; set; }

    public decimal? TotalIntoMoney { get; set; }

    public decimal? TotalQty { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ProjectID { get; set; }

    public int? ProductType { get; set; }

    public int? POKHID { get; set; }

    public string? GroupExport { get; set; }
}
