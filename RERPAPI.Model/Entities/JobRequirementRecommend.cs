using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu các đề xuất phương án tuyển dụng cho phiếu yêu cầu công việc
/// </summary>
public partial class JobRequirementRecommend
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID phiếu yêu cầu công việc
    /// </summary>
    public int? JobRequirementID { get; set; }

    /// <summary>
    /// ID người đề xuất phương án
    /// </summary>
    public int? RequesterID { get; set; }

    /// <summary>
    /// Ngày đề xuất phương án
    /// </summary>
    public DateTime? RequestDate { get; set; }

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
}
