using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng quản lý booking vé máy bay
/// </summary>
public partial class FlightBookingManagement
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID người đi
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Mục đích
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// ID dự án
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// Điểm đi
    /// </summary>
    public string? DepartureAddress { get; set; }

    /// <summary>
    /// Điểm đến
    /// </summary>
    public string? ArrivesAddress { get; set; }

    /// <summary>
    /// Ngày đi
    /// </summary>
    public DateTime? DepartureDate { get; set; }

    /// <summary>
    /// Giờ đi
    /// </summary>
    public DateTime? DepartureTime { get; set; }

    /// <summary>
    /// ID người đặt
    /// </summary>
    public int? EmployeeBookerID { get; set; }

    /// <summary>
    /// Ngày đặt
    /// </summary>
    public DateTime? BookedDate { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm (0: chưa xóa, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }
}
