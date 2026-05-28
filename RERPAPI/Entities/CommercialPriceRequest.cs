using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Báo giá thương mại
/// </summary>
public partial class CommercialPriceRequest
{
    public int ID { get; set; }

    /// <summary>
    /// RFQ No
    /// </summary>
    public string? RfqNo { get; set; }

    /// <summary>
    /// Request Seq
    /// </summary>
    public int? RequestSeq { get; set; }

    /// <summary>
    /// Item Code
    /// </summary>
    public string? ProductCode { get; set; }

    /// <summary>
    /// tên sản phẩm
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// Thông số kỹ thuật
    /// </summary>
    public string? Specification { get; set; }

    /// <summary>
    /// Mô tả
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// số lượng yêu cầu
    /// </summary>
    public decimal? Qty { get; set; }

    /// <summary>
    /// đơn vị tính ( pcs, kg, m)
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// số lượng đặt hàng tối thiểu
    /// </summary>
    public decimal? Moq { get; set; }

    /// <summary>
    /// ID người phụ trách mua hàng (Purchasing PIC)
    /// </summary>
    public int? PicPurID { get; set; }

    /// <summary>
    /// Tên người phụ trách mua hàng (Purchasing PIC)
    /// </summary>
    public string? PicPurName { get; set; }

    /// <summary>
    /// thời gian admin gửi (Thời gian yêu cầu)
    /// </summary>
    public DateTime? AdminSentAt { get; set; }

    /// <summary>
    /// thời gian pur gửi
    /// </summary>
    public DateTime? PurSentAt { get; set; }

    /// <summary>
    /// Hạn Purchasing phải có giá
    /// </summary>
    public DateOnly? QuoteDeadline { get; set; }

    /// <summary>
    /// Hạn Sales phải báo giá cho khách
    /// </summary>
    public DateOnly? SaleDeadline { get; set; }

    /// <summary>
    /// Ngày Sales đẩy request lên hệ thống
    /// </summary>
    public DateOnly? SalesPushedAt { get; set; }

    /// <summary>
    /// nhà cung cấp
    /// </summary>
    public string? Supplier { get; set; }

    /// <summary>
    /// đơn giá
    /// </summary>
    public string? UnitPrice { get; set; }

    /// <summary>
    /// phí vận chuyển
    /// </summary>
    public decimal? ShippingCost { get; set; }

    /// <summary>
    /// chi phí khác
    /// </summary>
    public decimal? OtherCost { get; set; }

    /// <summary>
    /// thuế VAT
    /// </summary>
    public decimal? Vat { get; set; }

    public string? Leadtime { get; set; }

    public string? SaleLeadtime { get; set; }

    /// <summary>
    /// tỉ lệ margin (%)
    /// </summary>
    public decimal? MarginRate { get; set; }

    /// <summary>
    /// đơn giá báo khách
    /// </summary>
    public decimal? SaleUnitPrice { get; set; }

    /// <summary>
    /// tổng giá báo khách
    /// </summary>
    public decimal? SaleTotalPrice { get; set; }

    /// <summary>
    /// trạng thái báo giá sale (0=Chưa, 1=Đã báo)
    /// </summary>
    public int? IsSaleQuoted { get; set; }

    /// <summary>
    /// Trạng thái báo giá Purchasing (0=Chưa, 1=Đã báo)
    /// </summary>
    public int? IsPurQuoted { get; set; }

    /// <summary>
    /// Trạng thái Trúng PO
    /// </summary>
    public int? IsPO { get; set; }

    /// <summary>
    /// Số lần báo giá
    /// </summary>
    public int? QuoteRound { get; set; }

    /// <summary>
    /// số tuần phát sinh request
    /// </summary>
    public int? WeekNo { get; set; }

    /// <summary>
    /// tháng phát sinh request
    /// </summary>
    public int? MonthNo { get; set; }

    /// <summary>
    /// Năm phát sinh request
    /// </summary>
    public int? YearNo { get; set; }

    /// <summary>
    /// Thời điểm Purchasing phản hồi
    /// </summary>
    public DateTime? PurRepliedAt { get; set; }

    /// <summary>
    /// Ghi chú request ban đầu (Sales)
    /// </summary>
    public string? RequestNote { get; set; }

    /// <summary>
    /// ghi chú giá nhập
    /// </summary>
    public string? ImportPriceNote { get; set; }

    /// <summary>
    /// Ghi chú báo giá cho khách
    /// </summary>
    public string? SaleNote { get; set; }

    /// <summary>
    /// ghi chú thay đổi giá
    /// </summary>
    public string? NoteReason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
