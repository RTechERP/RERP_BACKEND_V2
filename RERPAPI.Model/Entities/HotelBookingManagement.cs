using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng quản lý yêu cầu đặt phòng khách sạn
/// </summary>
public partial class HotelBookingManagement
{
    /// <summary>
    /// ID bản ghi, tự động tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Số thứ tự bản ghi
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// ID nhân viên yêu cầu đặt phòng
    /// </summary>
    public int? EmployeeRequestID { get; set; }

    /// <summary>
    /// Mục đích đặt phòng
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// ID dự án
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// Vị trí hoặc địa điểm khách sạn
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Ngày và giờ check-in
    /// </summary>
    public DateTime? CheckinDate { get; set; }

    /// <summary>
    /// Ngày và giờ check-out
    /// </summary>
    public DateTime? CheckOutDate { get; set; }

    /// <summary>
    /// ID nhân viên duyệt yêu cầu
    /// </summary>
    public int? EmployeeApproverID { get; set; }

    /// <summary>
    /// ID nhân viên thực hiện đặt phòng
    /// </summary>
    public int? EmployeeBookerID { get; set; }

    /// <summary>
    /// Ngày tạo yêu cầu đặt phòng
    /// </summary>
    public DateTime? DateRequest { get; set; }

    /// <summary>
    /// Ghi chú của yêu cầu đặt phòng
    /// </summary>
    public string? Note { get; set; }

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
