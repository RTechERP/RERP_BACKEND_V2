using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Danh mục file đính kèm lịch sử sửa chữa xe
/// </summary>
public partial class VehicleRepairHistoryFile
{
    /// <summary>
    /// ID của bản ghi (tự tăng)
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Số thứ tự
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// ID bảng VehicleRepairHistory
    /// </summary>
    public int? VehicleRepairHistoryID { get; set; }

    /// <summary>
    /// Tên file
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Đường dẫn nguyên bản
    /// </summary>
    public string? OriginPath { get; set; }

    /// <summary>
    /// Địa chỉ đường dẫn server
    /// </summary>
    public string? ServerPath { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Trạng thái (0: chưa xóa, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }
}
