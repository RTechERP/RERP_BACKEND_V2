using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu các phương án đề xuất booking vé máy bay
/// </summary>
public partial class FlightBookingProposal
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Hãng bay
    /// </summary>
    public string? Airline { get; set; }

    /// <summary>
    /// Chi phí
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Hành lý
    /// </summary>
    public string? Baggage { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm (0: chưa xóa, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Trạng thái duyệt (0: chưa duyệt, 1: đã duyệt, 2: không duyệt)
    /// </summary>
    public int? IsApprove { get; set; }

    /// <summary>
    /// ID người duyệt
    /// </summary>
    public int? ApproveID { get; set; }

    /// <summary>
    /// Lý do không duyệt
    /// </summary>
    public string? ReasonDecline { get; set; }

    public int? FlightBookingManagementID { get; set; }

    public bool? HCNSProposal { get; set; }
}
