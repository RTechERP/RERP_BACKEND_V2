using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu lịch sử các thao tác của phiếu đánh giá chuyển hợp đồng
/// </summary>
public partial class JobPerfomanceEvaluationNewLog
{
    public int ID { get; set; }

    /// <summary>
    /// ID phiếu đánh giá chuyển hợp đồng (JobPerfomanceEvaluationNew.ID)
    /// </summary>
    public int? JobPerfomanceEvaluationNewID { get; set; }

    /// <summary>
    /// ID nhân viên được đánh giá
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Loại thao tác: HR tạo phiếu, NLĐ/TBP/HR/BGĐ xác nhận, ...
    /// </summary>
    public string? ActionType { get; set; }

    /// <summary>
    /// Nội dung chi tiết của thao tác được ghi nhận
    /// </summary>
    public string? ContentLog { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Cờ đánh dấu xóa mềm: 0 - Đang sử dụng, 1 - Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }
}
