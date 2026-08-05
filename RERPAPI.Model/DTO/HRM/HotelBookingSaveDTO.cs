using System;
using System.Collections.Generic;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.HRM;

/// <summary>
/// DTO để lưu thông tin đăng ký phòng khách sạn (hỗ trợ nhiều phòng/người đi cùng lúc)
/// </summary>
public class HotelBookingSaveDTO
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

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
    /// Ngày check-in
    /// </summary>
    public DateTime? CheckinDate { get; set; }

    /// <summary>
    /// Ngày check-out
    /// </summary>
    public DateTime? CheckOutDate { get; set; }

    /// <summary>
    /// ID nhân viên duyệt
    /// </summary>
    public int? EmployeeApproverID { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// ID nhân viên đặt phòng (được gán tự động từ user hiện tại)
    /// </summary>
    public int? EmployeeBookerID { get; set; }

    /// <summary>
    /// Danh sách ID nhân viên đi cùng
    /// </summary>
    public List<int> TravelerIDs { get; set; } = new List<int>();

    /// <summary>
    /// Danh sách chi tiết người ở phòng
    /// </summary>
    public List<HotelBookingEmployee> Employees { get; set; } = new List<HotelBookingEmployee>();

    /// <summary>
    /// Danh sách các phương án đề xuất
    /// </summary>
    public List<HotelBookingProposal> Proposals { get; set; } = new List<HotelBookingProposal>();
}
