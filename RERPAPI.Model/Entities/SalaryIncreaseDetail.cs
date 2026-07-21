using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng chi tiết lưu danh sách nhân viên thuộc từng đợt tăng lương
/// </summary>
public partial class SalaryIncreaseDetail
{
    /// <summary>
    /// ID bản ghi, tự động tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID nhân viên được tăng lương
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Email của trưởng bộ phận nhận thông báo
    /// </summary>
    public string? EmailTBP { get; set; }

    /// <summary>
    /// Lương cơ bản hiện tại trước khi tăng
    /// </summary>
    public decimal? PreviousBaseSalary { get; set; }

    /// <summary>
    /// Lương cơ bản mới sau khi tăng
    /// </summary>
    public decimal? CurrentBaseSalary { get; set; }

    /// <summary>
    /// Ngày cập nhật bản ghi gần nhất
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật bản ghi gần nhất
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm: 0 - Chưa xóa, 1 - Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// ID đợt tăng lương thuộc bảng master SalaryIncrease
    /// </summary>
    public int? SalaryIncreaseID { get; set; }

    /// <summary>
    /// Trạng thái gửi hoặc nhận thông báo: 0 - Chưa nhận, 1 - Đã nhận
    /// </summary>
    public bool? IsSend { get; set; }
}
