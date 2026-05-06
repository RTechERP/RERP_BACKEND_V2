using System;
using System.Collections.Generic;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO.HRM;

/// <summary>
/// DTO để lưu thông tin đăng ký vé máy bay (hỗ trợ nhiều người đi cùng lúc)
/// </summary>
public class FlightBookingSaveDTO
{
    /// <summary>
    /// ID 
    /// </summary>
    public int ID { get; set; }

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
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// ID người đặt vé
    /// </summary>
    public int? EmployeeBookerID { get; set; }

    /// <summary>
    /// Danh sách người đi (Employee IDs)
    /// </summary>
    public List<int> TravelerIDs { get; set; } = new List<int>();

    /// <summary>
    /// Danh sách các phương án đề xuất
    /// </summary>
    public List<FlightBookingProposal> Proposals { get; set; } = new List<FlightBookingProposal>();
}
