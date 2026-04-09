using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Tờ trình phê duyệt tuyển dụng
/// </summary>
public partial class HRRecruitmentApprove
{
    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Ngày ban hành
    /// </summary>
    public DateTime? DateOfIssue { get; set; }

    /// <summary>
    /// Địa điểm ban hành
    /// </summary>
    public string? LocationOfIssue { get; set; }

    /// <summary>
    /// ID phòng ban
    /// </summary>
    public int? DepartmentID { get; set; }

    /// <summary>
    /// ID đơn xin tuyển dụng nhân sự
    /// </summary>
    public int? HRRecruitmentApplicationFormID { get; set; }

    /// <summary>
    /// Ngày bắt đầu thử việc
    /// </summary>
    public DateTime? DateStart { get; set; }

    /// <summary>
    /// Thời gian thử việc
    /// </summary>
    public string? ProbationPeriod { get; set; }

    /// <summary>
    /// Lương cơ bản
    /// </summary>
    public decimal? BasicSalary { get; set; }

    /// <summary>
    /// Lương thử việc
    /// </summary>
    public decimal? ProbationarySalary { get; set; }

    /// <summary>
    /// Người lập ký (employeeID)
    /// </summary>
    public int? EmployeeApprover { get; set; }

    /// <summary>
    /// TBP ký (employeeID)
    /// </summary>
    public int? TBPApprover { get; set; }

    /// <summary>
    /// BGD ký (EmployeeID)
    /// </summary>
    public int? BGDApprover { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người update
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày update
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Trạng thái xoá
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Tên người lập
    /// </summary>
    public string? EmployeeApproverName { get; set; }

    /// <summary>
    /// Tên TBP Ký
    /// </summary>
    public string? TBPApproverName { get; set; }

    /// <summary>
    /// Tên BGD Ký
    /// </summary>
    public string? BGDApproverName { get; set; }

    /// <summary>
    /// Trưởng phòng HCNS ký(employeeID)
    /// </summary>
    public int? HCNSApprove { get; set; }

    /// <summary>
    /// Tên Trưởng phòng HCNS ký
    /// </summary>
    public string? HCNSApproveName { get; set; }
}
