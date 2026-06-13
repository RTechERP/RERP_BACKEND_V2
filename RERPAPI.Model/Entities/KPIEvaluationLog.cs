using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu lịch sử các thao tác đánh giá KPI của nhân viên
/// </summary>
public partial class KPIEvaluationLog
{
    /// <summary>
    /// Khóa chính tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Mã kỳ đánh giá KPI
    /// </summary>
    public int? KPIExamID { get; set; }

    /// <summary>
    /// Mã nhân viên thực hiện hoặc liên quan đến thao tác đánh giá
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Loại thao tác thực hiện: Tạo mới, Cập nhật, Duyệt, Từ chối, Xóa...
    /// </summary>
    public string? ActionType { get; set; }

    /// <summary>
    /// Nội dung chi tiết của thao tác được ghi nhận
    /// </summary>
    public string? ContentLog { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Thời gian tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người cập nhật bản ghi gần nhất
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Thời gian cập nhật bản ghi gần nhất
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Cờ đánh dấu xóa mềm: 0 - Đang sử dụng, 1 - Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }
}
