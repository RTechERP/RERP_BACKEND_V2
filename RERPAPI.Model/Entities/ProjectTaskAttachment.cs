using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu chi tiết đính kèm (ảnh, video, link) công việc của dự án
/// </summary>
public partial class ProjectTaskAttachment
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
    /// Tên file hoặc tên link
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Đường dẫn file trên server hoặc đường dẫn link
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// ID của nhân viên tải file lên
    /// </summary>
    public int? EmployeeUploadID { get; set; }

    /// <summary>
    /// Ngày tải file lên
    /// </summary>
    public DateTime? UploadedDate { get; set; }

    /// <summary>
    /// 1: Files, 2: Links
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
    /// Trạng thái xóa mềm 
    /// </summary>
    public bool? IsDeleted { get; set; }
}
