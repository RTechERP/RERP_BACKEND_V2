using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ExpectedPayable
{
    public int ID { get; set; }

    /// <summary>
    /// Id phiếu nhập
    /// </summary>
    public int? BillImportID { get; set; }

    /// <summary>
    /// ID nhà cung cấp
    /// </summary>
    public int? SupplierSaleID { get; set; }

    /// <summary>
    /// Loại tiền
    /// </summary>
    public int? CurrencyID { get; set; }

    /// <summary>
    /// Nhân viên mua/ người giao
    /// </summary>
    public int? DeliverID { get; set; }

    /// <summary>
    /// Số hóa đơn
    /// </summary>
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Ngày hóa đơn
    /// </summary>
    public DateTime? InvoiceDate { get; set; }

    /// <summary>
    /// Ngày tới hạn
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Đơn giá
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Công nợ trong nước
    /// </summary>
    public decimal? DomesticPayable { get; set; }

    /// <summary>
    /// Công nợ nước ngoài
    /// </summary>
    public decimal? ForeignPayable { get; set; }

    /// <summary>
    /// Tiền hàng phát sinh
    /// </summary>
    public decimal? ArisingAmount { get; set; }

    /// <summary>
    /// Chi phí văn phòng
    /// </summary>
    public decimal? OfficeExpense { get; set; }

    /// <summary>
    /// Tiền thuế
    /// </summary>
    public decimal? TaxAmount { get; set; }

    public string? Note { get; set; }

    public bool? IsDeleted { get; set; }

    public int? PONCCID { get; set; }

    /// <summary>
    /// % Thanh toán
    /// </summary>
    public decimal? PaymentPercentage { get; set; }
}
