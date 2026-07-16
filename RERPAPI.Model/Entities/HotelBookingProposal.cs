using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng đề xuất thông tin phòng và chi phí đặt khách sạn
/// </summary>
public partial class HotelBookingProposal
{
    /// <summary>
    /// ID bản ghi, tự động tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Loại phòng
    /// </summary>
    public string? TypeRoom { get; set; }

    /// <summary>
    /// Số lượng phòng
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// Đơn giá phòng
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Thành tiền
    /// </summary>
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Đánh dấu HCNS đề xuất: 0 - Không, 1 - Có
    /// </summary>
    public bool IsHCNSProposal { get; set; }

    /// <summary>
    /// Lý do HCNS đề xuất
    /// </summary>
    public string? ReasonHCNSProposal { get; set; }

    /// <summary>
    /// ID bản ghi master trong bảng HotelBookingManagement
    /// </summary>
    public int? HotelBookingManagementID { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật bản ghi gần nhất
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật bản ghi gần nhất
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm: 0 - Chưa xóa, 1 - Đã xóa
    /// </summary>
    public bool IsDeleted { get; set; }

    public int? IsApprove { get; set; }

    public int? ApproveID { get; set; }

    public string? ReasonDecline { get; set; }
}
