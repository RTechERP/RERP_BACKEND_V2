using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class FlightBookingManagement
{
    /// <summary>
    /// ID của bảng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Trạng thái (0: Đã check vé, 1: Đã đặt vé)
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// ID nhân viên đặt vé
    /// </summary>
    public int EmployeeID { get; set; }

    /// <summary>
    /// ID trưởng bộ phận duyệt
    /// </summary>
    public int? ApproveID { get; set; }

    /// <summary>
    /// 0: Chưa duyệt, 1: Đã duyệt, 2: Không duyệt
    /// </summary>
    public int IsApprove { get; set; }

    public string? DeclineReason { get; set; }

    public DateTime? DepartureDate { get; set; }

    public DateTime? DepartureDateActual { get; set; }

    public string? DepartureAddress { get; set; }

    public string? DepartureAddressActual { get; set; }

    public bool IsRoundTrip { get; set; }

    public DateTime? ReturnDate { get; set; }

    public DateTime? ReturnDateActual { get; set; }

    public string? ReturnAddress { get; set; }

    public string? ReturnAddressActual { get; set; }

    public bool IsBaggage { get; set; }

    public decimal? BaggageWeight { get; set; }

    public string? BaggageDescription { get; set; }

    public decimal? TicketPrice { get; set; }

    public string? Note { get; set; }

    public bool IsSendMail { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
}
