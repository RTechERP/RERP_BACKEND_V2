using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Lưu danh sách file đính kèm của từng checklist trong Project Gate Step.
/// </summary>
public partial class ProjectGateStepFile
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Tên file gốc
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// Đường dẫn lưu file
    /// </summary>
    public string FilePath { get; set; } = null!;

    /// <summary>
    /// Kích thước file (Byte)
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// Kiểu MIME của file (image/png, application/pdf...)
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Đánh dấu xóa mềm (0: Chưa xóa, 1: Đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    public int? ProjectGateStepCheckListDetailLinkID { get; set; }

    public int? EmployeeID { get; set; }

    public int? Status { get; set; }
}
