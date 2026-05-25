using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu chi tiết loại công việc của dự án
/// </summary>
public partial class ProjectTaskType
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Tên loại công việc
    /// </summary>
    public string? TypeName { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Ngày cập nhật bản ghi
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Người cập nhật bản ghi
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Mã loại công việc
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// ID của phòng ban của công việc đó
    /// </summary>
    public int? DepartmentID { get; set; }

    /// <summary>
    /// Mã màu
    /// </summary>
    public string? Color { get; set; }
}
