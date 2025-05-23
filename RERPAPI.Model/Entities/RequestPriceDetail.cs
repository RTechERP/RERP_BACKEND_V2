using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Yêu cầu hỏi giá chi tiết
/// </summary>
public partial class RequestPriceDetail
{
    public long ID { get; set; }

    public int? RequestPriceID { get; set; }

    public long? ParentID { get; set; }

    /// <summary>
    /// ID vật tư, thiết bị, hàng hóa cần báo giá
    /// </summary>
    public int? PartID { get; set; }

    /// <summary>
    /// Mã vật tư thiết bị
    /// </summary>
    public string? PartCode { get; set; }

    /// <summary>
    /// Tên vật tư thiết bị
    /// </summary>
    public string? PartName { get; set; }

    /// <summary>
    /// ID hãng sản xuất
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// ID nhà cung cấp
    /// </summary>
    public int? SupplierID { get; set; }

    /// <summary>
    /// Tên nhà cung cấp
    /// </summary>
    public string? SupplierName { get; set; }

    /// <summary>
    /// Người hỏi giá, phụ trách
    /// </summary>
    public int? AskPriceID { get; set; }

    /// <summary>
    /// Khoảng thời gian hàng về dự kiến
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

    /// <summary>
    /// Ngày nhà cung cấp báo giá
    /// </summary>
    public DateTime? SupplierReplyDate { get; set; }

    /// <summary>
    /// Đơn vị tính
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ
    /// </summary>
    public string? CurrencyUnit { get; set; }

    public decimal? CurrencyRate { get; set; }

    public decimal? QtySet { get; set; }

    public decimal? QtyPS { get; set; }

    /// <summary>
    /// Số lượng
    /// </summary>
    public decimal? Qty { get; set; }

    /// <summary>
    /// đơn giá vật tư nhập
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Tổng tiền nhập hàng
    /// </summary>
    public decimal? TotalPrice { get; set; }

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

    /// <summary>
    /// Đơn giá sau chi phí
    /// </summary>
    public decimal? FinishPrice { get; set; }

    /// <summary>
    /// Tổng tiền cuối cùng kết thúc hỏi giá
    /// </summary>
    public decimal? FinishTotalPrice { get; set; }

    public decimal? PricePS { get; set; }

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

    /// <summary>
    /// Trạng thái hỏi giá: 0: chờ giá, 1: đã có giá
    /// </summary>
    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? STT { get; set; }

    /// <summary>
    /// độ ưu tiên
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// ngày deadline
    /// </summary>
    public DateTime? DeadLine { get; set; }

    public string? ProjectName { get; set; }

    public string? CustomerName { get; set; }

    public int? UserID { get; set; }

    public string? Note { get; set; }

    /// <summary>
    /// Lick sản phẩm khi được lấy trên mạng
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// add file theo định dạng bất kì
    /// </summary>
    public string? FileName { get; set; }

    public bool IsApproved { get; set; }

    public DateTime? AskDate { get; set; }

    public string? NoteSlowCheck { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }
}
