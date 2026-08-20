using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TravelRegistration
{
    public int ID { get; set; }

    public int EmployeeID { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public string? Department { get; set; }

    public string? PositionName { get; set; }

    public DateOnly? BirthDay { get; set; }

    public int? Age { get; set; }

    public string? Height { get; set; }

    public string? Gender { get; set; }

    public string? Relationship { get; set; }

    public string? Address { get; set; }

    public string? CCCD { get; set; }

    public DateTime? CCCDIssueDate { get; set; }

    public string? CCCDIssuePlace { get; set; }

    public string? PhoneNumber { get; set; }

    public string? DepartureLocation { get; set; }

    public DateTime? ConfirmDate { get; set; }

    public string? ConfirmBy { get; set; }

    public bool IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int OwnerEmployeeID { get; set; }

    public int? ConfirmStatus { get; set; }

    public bool? IsPublish { get; set; }

    public string? DangKyHLKGChieuDi { get; set; }

    public string? DangKyHLKGChieuVe { get; set; }

    public bool? DangKyVinwonders { get; set; }

    /// <summary>
    /// Đoàn
    /// </summary>
    public string? GroupNumber { get; set; }

    /// <summary>
    /// Ngày bay chuyến đi
    /// </summary>
    public DateTime? DepartureDate { get; set; }

    /// <summary>
    /// Mã chuyến bay chuyến đi
    /// </summary>
    public string? DepartureFlightCode { get; set; }

    /// <summary>
    /// Giờ bay chuyến đi
    /// </summary>
    public string? DepartureFlightTime { get; set; }

    /// <summary>
    /// Hành lý ký gửi chuyến đi
    /// </summary>
    public string? DepartureHLKG { get; set; }

    /// <summary>
    /// Ngày bay chuyến về
    /// </summary>
    public DateTime? ReturnDate { get; set; }

    /// <summary>
    /// Mã chuyến bay chuyến về
    /// </summary>
    public string? ReturnFlightCode { get; set; }

    /// <summary>
    /// Giờ bay chuyến về
    /// </summary>
    public string? ReturnFlightTime { get; set; }

    /// <summary>
    /// Hành lý ký gửi chuyến về
    /// </summary>
    public string? ReturnHLKG { get; set; }

    /// <summary>
    /// Xe tiễn VP HN đến sân bay Nội Bài / VP HCM đến sân bay Tân Sơn Nhất
    /// </summary>
    public string? XeVPSB { get; set; }

    /// <summary>
    /// Xe đón tại sân bay Cam Ranh
    /// </summary>
    public string? XeSBKS { get; set; }

    /// <summary>
    /// Xe đi Vinwonder
    /// </summary>
    public string? XeVinWonder { get; set; }

    /// <summary>
    /// Xe đi Gala Dinner
    /// </summary>
    public string? XeGalaDinner { get; set; }

    /// <summary>
    /// Xe tiễn khách sạn đến sân bay Cam Ranh
    /// </summary>
    public string? XeKSSB { get; set; }

    /// <summary>
    /// Xe đón sân bay Nội Bài đến VP HN / sân bay Tân Sơn Nhất đến VP HCM
    /// </summary>
    public string? XeSBVP { get; set; }

    /// <summary>
    /// Ngày đi Vinwonder
    /// </summary>
    public DateTime? DateDepartureVinWonder { get; set; }

    /// <summary>
    /// Số phòng
    /// </summary>
    public string? RoomNumber { get; set; }

    /// <summary>
    /// Mã phòng (cung cấp sau)
    /// </summary>
    public string? RommCode { get; set; }

    /// <summary>
    /// Loại giường
    /// </summary>
    public string? RoomType { get; set; }

    /// <summary>
    /// Ghi chú của BTC
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Xếp bàn Gala Dinner
    /// </summary>
    public string? TableNumberGala { get; set; }

    /// <summary>
    /// Ghi chú 2
    /// </summary>
    public string? Note2 { get; set; }

    /// <summary>
    /// Đồng thanh toán: Chính thức 1,5 triệu/người; Thử việc 2 triệu/người; Người thân từ 5 tuổi 4 triệu/người
    /// </summary>
    public string? TripCost { get; set; }

    /// <summary>
    /// Chiều cao từ 1m đi Vinwonder, hỗ trợ thêm 200.000 đồng/người
    /// </summary>
    public string? VinWonderCost { get; set; }

    /// <summary>
    /// Mua hành lý ký gửi
    /// </summary>
    public string? HLKGCost { get; set; }

    /// <summary>
    /// Tổng cộng CBNV thanh toán
    /// </summary>
    public string? TotalCost { get; set; }

    /// <summary>
    /// Công ty hỗ trợ: Hỗ trợ từ 5 tuổi; tự túc 1 bữa tối 200.000 đồng/người; tự túc 1 bữa trưa 200.000 đồng/người
    /// </summary>
    public string? SupportLunchCost { get; set; }

    /// <summary>
    /// Hỗ trợ tự túc vé máy bay
    /// </summary>
    public string? SupportFlightCost { get; set; }

    /// <summary>
    /// Tổng cộng công ty hỗ trợ
    /// </summary>
    public string? SupportTotalCost { get; set; }

    /// <summary>
    /// Tổng thanh toán theo từng người
    /// </summary>
    public string? SeptemberDeductionAmount { get; set; }

    /// <summary>
    /// Tổng thanh toán
    /// </summary>
    public string? TotalPaymentAmount { get; set; }
}
