using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu chi tiết log khi thay đổi thông tin công việc của dự án
/// </summary>
public partial class ProjectTaskLog
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID của bảng ProjectItem
    /// </summary>
    public int? ProjectTaskID { get; set; }

    /// <summary>
    /// Loại Log (người dùng có thể tự nhập tên loại log)
    /// </summary>
    public string? TypeLog { get; set; }

    /// <summary>
    /// Nội dung chi tiết log
    /// </summary>
    public string? ContentLog { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật bản ghi
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Ngươì cập nhật bản ghi
    /// </summary>
    public string UpdatedBy { get; set; } = null!;

    /// <summary>
    /// Trạng thái khóa mềm 
    /// </summary>
    public bool? IsDeleted { get; set; }
}
