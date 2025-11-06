using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillExportDetail
{
    public int ID { get; set; }

    /// <summary>
    /// Id master
    /// </summary>
    public int? ProductID { get; set; }

    public int? BillID { get; set; }

    /// <summary>
    /// tên sản phẩm
    /// </summary>
    public string? ProductFullName { get; set; }

    /// <summary>
    /// số lượng
    /// </summary>
    public decimal? Qty { get; set; }

    /// <summary>
    /// tên dự án
    /// </summary>
    public string? ProjectName { get; set; }

    public int? ExportID { get; set; }

    public string? Note { get; set; }

    public int? STT { get; set; }

    public decimal? TotalQty { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ProjectID { get; set; }

    public int? ProductType { get; set; }

    public int? POKHID { get; set; }

    public string? GroupExport { get; set; }

    public bool? IsInvoice { get; set; }

    public string? InvoiceNumber { get; set; }

    public string? SerialNumber { get; set; }

    public bool? ReturnedStatus { get; set; }

    public int? ProjectPartListID { get; set; }

    public int? TradePriceDetailID { get; set; }

    public int? POKHDetailID { get; set; }

    public string? Specifications { get; set; }

    public int? BillImportDetailID { get; set; }

    public decimal? TotalInventory { get; set; }

    public DateTime? ExpectReturnDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? BillExportDetail1 { get; set; }
}
