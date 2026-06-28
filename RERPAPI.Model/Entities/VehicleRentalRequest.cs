using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu thông tin yêu cầu thuê xe vận chuyển
/// </summary>
public partial class VehicleRentalRequest
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Ngày yêu cầu
    /// </summary>
    public DateTime? DateRequest { get; set; }

    /// <summary>
    /// Số thứ tự
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// ID phòng ban
    /// </summary>
    public int? DepartmentID { get; set; }

    /// <summary>
    /// Tên gói hàng
    /// </summary>
    public string? PackageName { get; set; }

    /// <summary>
    /// ID dự án
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// Số lượng kiện hàng
    /// </summary>
    public int? PackageQuantity { get; set; }

    /// <summary>
    /// Chiều dài gói hàng (cm)
    /// </summary>
    public decimal? PackageLengthCm { get; set; }

    /// <summary>
    /// Chiều rộng gói hàng (cm)
    /// </summary>
    public decimal? PackageWidthCm { get; set; }

    /// <summary>
    /// Chiều cao gói hàng (cm)
    /// </summary>
    public decimal? PackageHeightCm { get; set; }

    /// <summary>
    /// Cân nặng gói hàng (kg)
    /// </summary>
    public decimal? PackageWeightKg { get; set; }

    /// <summary>
    /// Địa điểm xuất phát
    /// </summary>
    public string? DepartureLocation { get; set; }

    /// <summary>
    /// Địa điểm cần đến
    /// </summary>
    public string? AddressLocation { get; set; }

    /// <summary>
    /// Khoảng cách (km)
    /// </summary>
    public decimal? DistanceKm { get; set; }

    /// <summary>
    /// Tên đơn vị vận chuyển
    /// </summary>
    public string? NameNCC { get; set; }

    /// <summary>
    /// Chi phí
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// ID nhân viên yêu cầu
    /// </summary>
    public int? EmployeeRequestID { get; set; }

    /// <summary>
    /// ID người đặt
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm (0: Chưa xóa, 1: Đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }

    public int? VehicleType { get; set; }
}
