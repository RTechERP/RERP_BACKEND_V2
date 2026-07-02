using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu thông tin yêu cầu làm visa công tác
/// </summary>
public partial class BusinessVisaRequest
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Số thứ tự
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// Loại đối tượng (1: CBNV, 2: Đối tác)
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// ID nhân viên nếu Type = 1
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Họ và tên
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Ngày sinh
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Giới tính (1: Nam, 2: Nữ)
    /// </summary>
    public int? Gender { get; set; }

    /// <summary>
    /// Quốc tịch
    /// </summary>
    public string? Nation { get; set; }

    /// <summary>
    /// Số hộ chiếu
    /// </summary>
    public string? HoChieu { get; set; }

    /// <summary>
    /// Nghề nghiệp
    /// </summary>
    public string? NgheNghiep { get; set; }

    /// <summary>
    /// Tên công ty
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Điểm đến
    /// </summary>
    public string? Destination { get; set; }

    /// <summary>
    /// Thời gian công tác từ
    /// </summary>
    public DateTime? BusinessTripFromDate { get; set; }

    /// <summary>
    /// Thời gian công tác đến
    /// </summary>
    public DateTime? BusinessTripToDate { get; set; }

    /// <summary>
    /// Chi phí làm visa
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Thời gian có visa
    /// </summary>
    public string? VisaIssueDate { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Trạng thái
    /// </summary>
    public string? Status { get; set; }

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
    /// Trạng thái xóa mềm (0: Chưa xóa, 1: Đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }
}
