using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectItem
{
    public int ID { get; set; }

    public int? Status { get; set; }

    public string? STT { get; set; }

    public int? UserID { get; set; }

    public int? ProjectID { get; set; }

    public string? Mission { get; set; }

    public DateTime? PlanStartDate { get; set; }

    public DateTime? PlanEndDate { get; set; }

    public DateTime? ActualStartDate { get; set; }

    public DateTime? ActualEndDate { get; set; }

    public string? Note { get; set; }

    public decimal? TotalDayPlan { get; set; }

    public decimal? PercentItem { get; set; }

    public int? ParentID { get; set; }

    public decimal? TotalDayActual { get; set; }

    /// <summary>
    /// 1:Hạng mục quá hạn,
    /// 0: Hạng mục đúng hạn
    /// </summary>
    public int? ItemLate { get; set; }

    public decimal? TimeSpan { get; set; }

    public int? TypeProjectItem { get; set; }

    public decimal? PercentageActual { get; set; }

    /// <summary>
    /// Người giao công việc
    /// </summary>
    public int? EmployeeIDRequest { get; set; }

    /// <summary>
    /// Ngày update kết thúc thực tế
    /// </summary>
    public DateTime? UpdatedDateActual { get; set; }

    /// <summary>
    /// 0: Chờ duyệt kế hoạch; 1:Leader duyệt kế hoạch; 2:Chờ duyệt thực tế; 3:Leader Duyệt thực tế
    /// </summary>
    public int? IsApproved { get; set; }

    public string? Code { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsUpdateLate { get; set; }

    public string? ReasonLate { get; set; }

    public DateTime? UpdatedDateReasonLate { get; set; }

    public bool? IsApprovedLate { get; set; }

    /// <summary>
    /// lưu ID người yêu cầu lấy từ bảng Employee, nếu  = 0 thì là tên KH
    /// </summary>
    public int? EmployeeRequestID { get; set; }

    /// <summary>
    /// lưu tên người yêu cầu lấy từ bảng Employee, nếu  = 0 thì là tên KH
    /// </summary>
    public string? EmployeeRequestName { get; set; }
    public bool IsDeleted { get; set; }
}
