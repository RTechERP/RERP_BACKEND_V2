using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Báo giá
/// </summary>
public partial class Quotation
{
    public int ID { get; set; }

    public int? RequestPriceID { get; set; }

    public int? ProjectID { get; set; }

    /// <summary>
    /// Khách hàng
    /// </summary>
    public int? CustomerID { get; set; }

    /// <summary>
    /// Nhân viên sale phụ trách
    /// </summary>
    public int? SaleID { get; set; }

    public int? CurrencyID { get; set; }

    public string? POCode { get; set; }

    /// <summary>
    /// Đã được duyệt hay chưa
    /// </summary>
    public bool? IsApproved { get; set; }

    /// <summary>
    /// 0: báo giá bình thường, 1: Báo giá hàng nhập khẩu(cognex)
    /// </summary>
    public int? QuotationType { get; set; }

    /// <summary>
    /// 0: Chưa duyệt,1:Đã duyệt ,2:Đã báo khách,3:Pending,4:Đã có PO
    /// </summary>
    public int? QuotationStatus { get; set; }

    /// <summary>
    /// Số báo giá
    /// </summary>
    public string? QuotationCode { get; set; }

    /// <summary>
    /// Ngày báo giá
    /// </summary>
    public DateTime? QuotationDate { get; set; }

    /// <summary>
    /// Tên người liên hệ
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// Điện thoại người liên hệ
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// Địa chỉ email người liên hệ
    /// </summary>
    public string? ContactEmail { get; set; }

    public string? TotalName { get; set; }

    public decimal? Qty { get; set; }

    /// <summary>
    /// Số lượng set
    /// </summary>
    public decimal? QtySet { get; set; }

    public decimal? VAT { get; set; }

    /// <summary>
    /// Đơn giá / 1set
    /// </summary>
    public decimal? PricePS { get; set; }

    /// <summary>
    /// Đơn giá / 1set
    /// </summary>
    public decimal? PricePSVAT { get; set; }

    public decimal? PriceVT { get; set; }

    public decimal? TotalVT { get; set; }

    public decimal? Price { get; set; }

    /// <summary>
    /// Tổng tiền báo giá
    /// </summary>
    public decimal? TotalPrice { get; set; }

    public decimal? PriceVAT { get; set; }

    /// <summary>
    /// Tổng tiền báo giá
    /// </summary>
    public decimal? TotalPriceVAT { get; set; }

    /// <summary>
    /// Chi phí bốc dỡ vận chuyển
    /// </summary>
    public decimal? DeliveryCost { get; set; }

    /// <summary>
    /// Chi phí ngân hàng
    /// </summary>
    public decimal? BankCost { get; set; }

    /// <summary>
    /// Tổng tiền chi phí hải quan
    /// </summary>
    public decimal? CustomsCost { get; set; }

    public decimal? Rate { get; set; }

    public string? Note { get; set; }

    public string? DeliveryPeriod { get; set; }

    public string? DeliveryFees { get; set; }

    public string? Validity { get; set; }

    public string? CustomClearance { get; set; }

    public string? Warranty { get; set; }

    public string? Payment { get; set; }

    public string? PlaceDelivery { get; set; }

    public string? Adjusting { get; set; }

    public bool? IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? TradePriceID { get; set; }
}
