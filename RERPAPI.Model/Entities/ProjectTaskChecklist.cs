using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu chi tiết checklist công việc của dự án
/// </summary>
public partial class ProjectTaskChecklist
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
    /// Nội dung của checklist 
    /// </summary>
    public string? ChecklistTitle { get; set; }

    /// <summary>
    /// Vị trí của CheckList
    /// </summary>
    public int? OrderIndex { get; set; }

    /// <summary>
    /// Trạng thái của checklist xem nó đã hoàn thành hày chưa
    /// </summary>
    public bool? IsDone { get; set; }

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
    /// Trạng thái khóa mềm
    /// </summary>
    public bool? IsDeleted { get; set; }
}
