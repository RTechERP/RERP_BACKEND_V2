using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng quản lý phòng ban/bộ phận chấm điểm 5S
/// </summary>
public partial class FiveSDepartment
{
    /// <summary>
    /// ID bản ghi (Khóa chính, tự tăng)
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Số thứ tự hiển thị
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// Mã phòng ban/bộ phận 5S
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Tên phòng ban/bộ phận 5S
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Mô tả chi tiết phòng ban/bộ phận
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ngày cập nhật dữ liệu
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật dữ liệu
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Cờ xóa mềm (0: Chưa xóa, 1: Đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }
}
