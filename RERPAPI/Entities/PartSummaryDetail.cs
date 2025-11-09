using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class PartSummaryDetail
{
    public long ID { get; set; }

    public int? PartID { get; set; }

    /// <summary>
    /// ID yêu cầu hỏi giá
    /// </summary>
    public int? RequestID { get; set; }

    /// <summary>
    /// Báo giá
    /// </summary>
    public int? QuotationID { get; set; }

    /// <summary>
    /// PO khách hàng
    /// </summary>
    public int? POCustomerID { get; set; }

    /// <summary>
    /// PO nhà cung cấp
    /// </summary>
    public int? POSupplierID { get; set; }

    /// <summary>
    /// Dự án
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// Nhà cung cấp
    /// </summary>
    public int? SupplierID { get; set; }

    /// <summary>
    /// ID hãng sản xuất
    /// </summary>
    public int? ManufacturerID { get; set; }

    /// <summary>
    /// Vật tư thiết bị cha
    /// </summary>
    public int? ParentID { get; set; }

    /// <summary>
    /// Thuộc thiết bị sản xuất nào trong dự án
    /// </summary>
    public int? ProjectModuleID { get; set; }

    /// <summary>
    /// Sale phụ trách
    /// </summary>
    public int? SaleID { get; set; }

    /// <summary>
    /// Người phụ trách mua
    /// </summary>
    public int? BuyPersonID { get; set; }

    /// <summary>
    /// Người hỏi giá
    /// </summary>
    public int? AskPriceID { get; set; }

    /// <summary>
    /// Mã vật tư thiết bị
    /// </summary>
    public string? PartCode { get; set; }

    /// <summary>
    /// Tên vật tư thiết bị
    /// </summary>
    public string? PartName { get; set; }

    /// <summary>
    /// Mã vật tư hàng hóa theo RTC
    /// </summary>
    public string? PartCodeRTC { get; set; }

    /// <summary>
    /// Tên vật tư hàng hóa theo RTC
    /// </summary>
    public string? PartNameRTC { get; set; }

    /// <summary>
    /// Số lượng
    /// </summary>
    public decimal? Qty { get; set; }

    /// <summary>
    /// Đơn vị tính
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ
    /// </summary>
    public string? CurrencyUnit { get; set; }

    /// <summary>
    /// vat
    /// </summary>
    public decimal? VAT { get; set; }

    /// <summary>
    /// Giá vật tư tiền việt
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Đơn giá sau VAT
    /// </summary>
    public decimal? PriceVAT { get; set; }

    /// <summary>
    /// Tổng tiền
    /// </summary>
    public decimal? TotalPrice { get; set; }

    /// <summary>
    /// Tổng tiền sau VAT
    /// </summary>
    public decimal? TotalPriceVAT { get; set; }

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
    /// Thuế nhập khẩu (%)
    /// </summary>
    public decimal? VATCurrencyPercent { get; set; }

    /// <summary>
    /// Chi phí thuể nhập khẩu trên một con vật tư
    /// </summary>
    public decimal? VATCurrencyPrice { get; set; }

    /// <summary>
    /// Tổng tiền chi phí nhập khẩu
    /// </summary>
    public decimal? VATCurrencyCost { get; set; }

    /// <summary>
    /// Tổng tiền chi phí hải quan
    /// </summary>
    public decimal? CustomsCost { get; set; }

    /// <summary>
    /// Tổng tiền cuối cùng kết thúc hỏi giá
    /// </summary>
    public decimal? FinishTotalPrice { get; set; }

    public decimal? PriceQuotation { get; set; }

    public decimal? TotalPriceQuotation { get; set; }

    /// <summary>
    /// Tên người liên hệ nhà cung cấp
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// Điện thoại người liên hệ nhà cung cấp
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// Email người liên hệ nhà cung cấp
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Ngày yêu cầu hỏi giá từ sale
    /// </summary>
    public DateTime? DateRequestPrice { get; set; }

    /// <summary>
    /// Thời gian hỏi xong giá
    /// </summary>
    public DateTime? DateReplyPrice { get; set; }

    /// <summary>
    /// Khoảng thời gian hàng về dự kiến lúc hỏi giá
    /// </summary>
    public string? PeriodExpected { get; set; }

    /// <summary>
    /// Ngày hàng về dự kiến nhanh nhất lúc hỏi giá
    /// </summary>
    public decimal? MinDay { get; set; }

    /// <summary>
    /// Ngày hàng về dự kiến lâu nhất lúc hỏi giá
    /// </summary>
    public decimal? MaxDay { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
