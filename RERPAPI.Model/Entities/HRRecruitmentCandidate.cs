using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HRRecruitmentCandidate
{
    public int ID { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public int? EmployeeChucVuHDID { get; set; }

    public string? FileCVName { get; set; }

    /// <summary>
    /// Đợt tuyển dụng
    /// </summary>
    public int? HrHiringRequestID { get; set; }

    /// <summary>
    /// Ngày ứng tuyển
    /// </summary>
    public DateTime? DateApply { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Trạng thái ứng viên
    /// </summary>
    public int? Status { get; set; }

    public string? Note { get; set; }

    public int? STT { get; set; }

    public string? ServerPath { get; set; }

    public string? FullName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public int? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public DateTime? SendMailTime { get; set; }

    /// <summary>
    /// 1. Đã gửi mail vòng 1 2. đã gửi mail vòng 2
    /// </summary>
    public int? StatusMail { get; set; }

    public DateTime? DateInterview { get; set; }

    /// <summary>
    /// Chức vụ cần tuyển
    /// </summary>
    public string? PositionName { get; set; }

    /// <summary>
    /// Hạn phản hồi mail
    /// </summary>
    public DateTime? DeadlineFeedbackMail { get; set; }

    /// <summary>
    /// Người phỏng vấn
    /// </summary>
    public int? InterviewerID { get; set; }
}
