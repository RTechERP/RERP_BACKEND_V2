using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu các loại thông báo trong hệ thống
/// </summary>
public partial class NotificationType
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Tên loại thông báo
    /// </summary>
    public string? TypeName { get; set; }

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
    /// Đánh dấu xóa mềm
    /// </summary>
    public bool? IsDeleted { get; set; }
}
