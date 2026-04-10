using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng danh sách email không gửi mail
/// </summary>
public partial class ProjectTaskEmailBand
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID của bảng Employee
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Email công ty của nhân viên muốn hủy gửi mail
    /// </summary>
    public string? EmployeeEmail { get; set; }

    /// <summary>
    /// Trạng thái hoạt động của email
    /// </summary>
    public bool? IsActive { get; set; }

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
