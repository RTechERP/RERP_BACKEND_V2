using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng master lưu thông tin các đợt tăng lương
/// </summary>
public partial class SalaryIncrease
{
    /// <summary>
    /// ID bản ghi, tự động tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Mã đợt tăng lương
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Tên đợt tăng lương
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Ngày bắt đầu có hiệu lực của đợt tăng lương
    /// </summary>
    public DateTime? EffectiveDate { get; set; }

    /// <summary>
    /// Ngày cập nhật bản ghi gần nhất
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật bản ghi gần nhất
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm: 0 - Chưa xóa, 1 - Đã xóa
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Tháng bắt đầu áp dụng đợt tăng lương, ví dụ: T6/2026
    /// </summary>
    public string? MonthFrom { get; set; }

    /// <summary>
    /// Tháng kết thúc áp dụng đợt tăng lương, ví dụ: T7/2026
    /// </summary>
    public string? MonthTo { get; set; }
}
