using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu nhân viên tham gia công việc của dự án
/// </summary>
public partial class ProjectTaskEmployee
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
    /// ID của bảng Employee
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// 1: assignee, 2: related
    /// </summary>
    public int? Type { get; set; }

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
