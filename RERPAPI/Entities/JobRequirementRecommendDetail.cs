using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Bảng chi tiết các phương án đề xuất cho phiếu yêu cầu công việc
/// </summary>
public partial class JobRequirementRecommendDetail
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID liên kết tới bảng JobRequirementRecommend
    /// </summary>
    public int? JobRequirementRecommendID { get; set; }

    /// <summary>
    /// Tên dịch vụ / hạng mục đề xuất
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// Tên nhà cung cấp
    /// </summary>
    public string? Supplier { get; set; }

    /// <summary>
    /// Thông tin liên hệ nhà cung cấp
    /// </summary>
    public string? Contact { get; set; }

    /// <summary>
    /// Đơn giá
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Thành tiền
    /// </summary>
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// Ghi chú thêm
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người cập nhật bản ghi
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật bản ghi
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm: 0 - Chưa xóa, 1 - Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Trạng thái duyệt: 0 - Chưa duyệt, 1 - Đã duyệt, 2 - Hủy duyệt
    /// </summary>
    public int? IsApproved { get; set; }

    /// <summary>
    /// Lý do không duyệt
    /// </summary>
    public string? DisapprovalReason { get; set; }

    /// <summary>
    /// Ngày duyệt
    /// </summary>
    public DateTime? ApprovalDate { get; set; }

    /// <summary>
    /// ID người duyệt
    /// </summary>
    public int? ApproverID { get; set; }
}
