using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Báo giá chi tiết
/// </summary>
public partial class QuotationDetail
{
    public long ID { get; set; }

    public long? ParentID { get; set; }

    public int? QuotationID { get; set; }

    public int? ManufacturerID { get; set; }

    /// <summary>
    /// ID nhà cung cấp
    /// </summary>
    public int? SupplierID { get; set; }

    public long? RequestPriceDetailID { get; set; }

    public int? PartID { get; set; }

    public string? PartCode { get; set; }

    public string? PartName { get; set; }

    public string? PartCodeRTC { get; set; }

    public string? PartNameRTC { get; set; }

    /// <summary>
    /// Khoảng thời gian hàng về dự kiến
    /// </summary>
    public string? PeriodExpected { get; set; }

    public string? Unit { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ
    /// </summary>
    public string? CurrencyUnit { get; set; }

    public decimal? CurrencyRate { get; set; }

    /// <summary>
    /// Số set
    /// </summary>
    public decimal? QtySet { get; set; }

    /// <summary>
    /// Số lượng / 1 set
    /// </summary>
    public decimal? QtyPS { get; set; }

    /// <summary>
    /// Tổng số lượng
    /// </summary>
    public decimal? Qty { get; set; }

    /// <summary>
    /// vat
    /// </summary>
    public decimal? VAT { get; set; }

    /// <summary>
    /// Tiền vat
    /// </summary>
    public decimal? PriceVAT { get; set; }

    /// <summary>
    /// Tổng tiền VAT
    /// </summary>
    public decimal? TotalVAT { get; set; }

    /// <summary>
    /// Đơn giá vật tư theo ngoại tệ
    /// </summary>
    public decimal? PriceCurrency { get; set; }

    /// <summary>
    /// Tổng tiền ngoại tệ
    /// </summary>
    public decimal? TotalPriceCurrency { get; set; }

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

    /// <summary>
    /// Thuế nhập khẩu (%)
    /// </summary>
    public decimal? TaxImportPercent { get; set; }

    /// <summary>
    /// Chi phí thuể nhập khẩu trên một con vật tư
    /// </summary>
    public decimal? TaxImporPrice { get; set; }

    /// <summary>
    /// Tổng tiền chi phí nhập khẩu
    /// </summary>
    public decimal? TaxImporTotal { get; set; }

    public decimal? PricePS { get; set; }

    /// <summary>
    /// Đơn giá sau chi phí
    /// </summary>
    public decimal? FinishPrice { get; set; }

    /// <summary>
    /// Tổng tiền cuối cùng sau chi phí
    /// </summary>
    public decimal? FinishTotalPrice { get; set; }

    public decimal? PriceVT { get; set; }

    public decimal? TotalVT { get; set; }

    /// <summary>
    /// Đơn giá báo cũ
    /// </summary>
    public decimal? PriceOld { get; set; }

    /// <summary>
    /// Đơn giá báo khách
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Tổng tiền báo khách
    /// </summary>
    public decimal? TotalPrice { get; set; }

    /// <summary>
    /// Tên người liên hệ
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// Điện thoại người liên hệ
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// Email người liên hệ
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Website liên hệ
    /// </summary>
    public string? ContactWebsite { get; set; }

    public string? Delivery { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Maker { get; set; }

    public string? STT { get; set; }

    public int? TradePriceDetailID { get; set; }
}
