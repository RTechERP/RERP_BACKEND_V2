using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class DailyReportAccounting
{
    public int ID { get; set; }

    /// <summary>
    /// Nhân viên/Họ tên (FK)
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Ngày báo cáo
    /// </summary>
    public DateTime? ReportDate { get; set; }

    /// <summary>
    /// Việc đã làm
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Kết quả/Tình trạng
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// Kế hoạch tiếp theo
    /// </summary>
    public string? NextPlan { get; set; }

    /// <summary>
    /// Tồn đọng/Vướng mắc
    /// </summary>
    public string? PendingIssues { get; set; }

    /// <summary>
    /// Phát sinh gấp cần xử lý
    /// </summary>
    public string? Urgent { get; set; }

    /// <summary>
    /// Lỗi/Sai phạm/Bị nhắc nhở
    /// </summary>
    public string? MistakeOrViolation { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
