using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Lưu lịch sử log các thao tác liên quan đến đơn đặt đồ ăn của nhân viên
/// </summary>
public partial class EmployeeFoodOrderLog
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID đơn đặt đồ ăn của nhân viên
    /// </summary>
    public int? EmployeeFoodOrderID { get; set; }

    /// <summary>
    /// Loại log (tạo mới, cập nhật, hủy...)
    /// </summary>
    public string? TypeLog { get; set; }

    /// <summary>
    /// Nội dung chi tiết của log
    /// </summary>
    public string? ContentLog { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Đánh dấu xóa mềm (0: hoạt động, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }
}
