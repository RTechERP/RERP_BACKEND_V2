using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu thông tin các đợt đăng ký nghỉ của nhân viên
/// </summary>
public partial class EmployeeOnLeavePhase
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID nhân viên đăng ký
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Mã đợt đăng ký
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Ngày đăng ký
    /// </summary>
    public DateTime? DateRegister { get; set; }

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
