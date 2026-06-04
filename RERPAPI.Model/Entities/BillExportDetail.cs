using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillExportDetail
{
    public int ID { get; set; }

    /// <summary>
    /// ID sản phẩm
    /// </summary>
    public int? ProductID { get; set; }

    /// <summary>
    /// ID master
    /// </summary>
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

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Số thứ tự
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// Tổng số lượng
    /// </summary>
    public decimal? TotalQty { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// ID dự án
    /// </summary>
    public int? ProjectID { get; set; }

    public int? ProductType { get; set; }

    /// <summary>
    /// ID pokh
    /// </summary>
    public int? POKHID { get; set; }

    public string? GroupExport { get; set; }

    /// <summary>
    /// Hóa đơn
    /// </summary>
    public bool? IsInvoice { get; set; }

    /// <summary>
    /// Số hóa đơn
    /// </summary>
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Số serial
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Trạng thái trả
    /// </summary>
    public bool? ReturnedStatus { get; set; }

    /// <summary>
    /// ID DMVT
    /// </summary>
    public int? ProjectPartListID { get; set; }

    public int? TradePriceDetailID { get; set; }

    /// <summary>
    /// ID chi tiết pokh
    /// </summary>
    public int? POKHDetailID { get; set; }

    /// <summary>
    /// Thông số/Model
    /// </summary>
    public string? Specifications { get; set; }

    /// <summary>
    /// ID phiếu nhập trả
    /// </summary>
    public int? BillImportDetailID { get; set; }

    public decimal? TotalInventory { get; set; }

    /// <summary>
    /// Ngày dự kiến trả
    /// </summary>
    public DateTime? ExpectReturnDate { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Phản hồi khách hàng
    /// </summary>
    public string? CustomerResponse { get; set; }
}
