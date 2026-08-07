using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng quản lý các bước/tiến độ được cấu hình cho dự án
/// </summary>
public partial class ProjectGateStepLink
{
    /// <summary>
    /// Khóa chính tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID dự án đang cấu hình
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// ID bước chuẩn (Master Step) liên kết để lấy tên bước và nội dung
    /// </summary>
    public int? ProjectGateStepID { get; set; }

    /// <summary>
    /// ID loại dự án, lưu để truy vấn nhanh
    /// </summary>
    public int? ProjectTypeID { get; set; }

    /// <summary>
    /// Ngày bắt đầu thực hiện công việc
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// Trạng thái lặp lại của công việc (0: Không, 1: Có)
    /// </summary>
    public bool? IsRepeat { get; set; }

    public int? ProjectTaskID { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? CreatedBy { get; set; }

    public int? ProjectGateStepTemplateID { get; set; }

    public int? DepartmentID { get; set; }

    public bool? IsApproved { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public int? ParentID { get; set; }

    public string? Content { get; set; }

    public int? SortOrder { get; set; }

    public string? ActualContent { get; set; }

    public DateTime? DateEnd { get; set; }
}
