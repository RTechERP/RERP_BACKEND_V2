using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu chi tiết duyệt công việc của dự án
/// </summary>
public partial class ProjectTaskApprove
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
    /// Trạng thái duyệt xem có duyệt hay không
    /// </summary>
    public bool? IsApprove { get; set; }

    /// <summary>
    /// Nội dung đánh giá 
    /// </summary>
    public string? Review { get; set; }

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
    /// Trạng thái xóa mềm 
    /// </summary>
    public bool? IsDeleted { get; set; }
}
