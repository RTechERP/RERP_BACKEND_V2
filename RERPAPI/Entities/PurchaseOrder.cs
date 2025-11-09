using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class PurchaseOrder
{
    public int ID { get; set; }

    /// <summary>
    /// Nhà cung cấp
    /// </summary>
    public int? SupplierID { get; set; }

    /// <summary>
    /// Người phụ trách mua
    /// </summary>
    public int? BuyPersonID { get; set; }

    /// <summary>
    /// Số PO
    /// </summary>
    public string? PurchaseOrderCode { get; set; }

    /// <summary>
    /// Ngày phát sinh PO
    /// </summary>
    public DateTime? PODate { get; set; }

    /// <summary>
    /// Khoảng thời gian hàng về dự kiến
    /// </summary>
    public string? PeriodExpected { get; set; }

    /// <summary>
    /// Tổng tiền PO chưa VAT
    /// </summary>
    public decimal? TotalPrice { get; set; }

    /// <summary>
    /// Tổng tiền VAT
    /// </summary>
    public decimal? TotalVAT { get; set; }

    /// <summary>
    /// Tổng tiền PO có VAT
    /// </summary>
    public decimal? FinishPrice { get; set; }

    /// <summary>
    /// Tên người liên hệ nhà cung cấp
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// Điện thoại người liên hệ nhà cung cấp
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// Địa chỉ email người liên hệ nhà cung cấp
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Quy tắc thanh toán
    /// </summary>
    public string? Payment { get; set; }

    /// <summary>
    /// Ngày giao hàng và vận chuyển
    /// </summary>
    public string? DateAndDelivery { get; set; }

    public string? DeliveryAndFees { get; set; }

    public string? PlaceOfDelivery { get; set; }

    public int? POStatus { get; set; }

    public bool? IsApproved { get; set; }

    public int? ApprovedID { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
