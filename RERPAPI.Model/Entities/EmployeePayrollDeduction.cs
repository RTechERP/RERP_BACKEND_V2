using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu thông tin tiền phạt (deduction) của nhân viên theo từng lần
/// </summary>
public partial class EmployeePayrollDeduction
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID nhân viên
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Ngày phát sinh tiền phạt
    /// </summary>
    public DateTime? DeductionDate { get; set; }

    /// <summary>
    /// Lý do phạt
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Số tiền bị trừ (VNĐ)
    /// </summary>
    public decimal? DeductionAmount { get; set; }

    /// <summary>
    /// Loại phạt: 1.Đi muộn, 2.Quên chấm công, 3.Đăng ký nghỉ, 4.Khác
    /// </summary>
    public int? DeductionType { get; set; }

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
    /// Trạng thái xóa mềm: 0.Chưa xóa, 1.Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }

    public string? DeductionTypeName { get; set; }
}
