using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu danh sách nhân viên, khách hoặc đối tác sử dụng phòng khách sạn
/// </summary>
public partial class HotelBookingEmployee
{
    /// <summary>
    /// ID bản ghi, tự động tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Loại đối tượng: 1 - Cán bộ nhân viên công ty, 2 - Khách hoặc đối tác
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// ID nhân viên; có thể để trống đối với khách hoặc đối tác
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Họ và tên nhân viên, khách hoặc đối tác
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// ID bản ghi master trong bảng HotelBookingManagement
    /// </summary>
    public int? HotelBookingManagementID { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật bản ghi gần nhất
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật bản ghi gần nhất
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm: 0 - Chưa xóa, 1 - Đã xóa
    /// </summary>
    public bool IsDeleted { get; set; }
}
