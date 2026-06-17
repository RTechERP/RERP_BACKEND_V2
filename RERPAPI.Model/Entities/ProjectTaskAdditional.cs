using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu phát sinh cho công việc của dự án
/// </summary>
public partial class ProjectTaskAdditional
{
    /// <summary>
    /// Để lưu ID của bảng ProjectItem
    /// </summary>
    public int? ProjectTaskID { get; set; }

    /// <summary>
    /// Nội dung cho phát sinh
    /// </summary>
    public string? Description { get; set; }

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
    /// Người cập nhật bản ghi
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Đánh dấu xóa mềm
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }
}
