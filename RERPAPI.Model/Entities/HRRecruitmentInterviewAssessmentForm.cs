using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Phiếu đánh giá kết quả phỏng vấn
/// </summary>
public partial class HRRecruitmentInterviewAssessmentForm
{
    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID người phỏng vấn
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Ngày phỏng vấn
    /// </summary>
    public DateTime? DateOfInterview { get; set; }

    /// <summary>
    /// đánh giá chung
    /// </summary>
    public int? OverrallImpression { get; set; }

    /// <summary>
    /// Note đánh giá chung
    /// 
    /// </summary>
    public string? OverrallImpressionNote { get; set; }

    /// <summary>
    /// Trình độ
    /// </summary>
    public int? Qualifications { get; set; }

    /// <summary>
    /// Note trình độ
    /// 
    /// </summary>
    public string? QualificationsNote { get; set; }

    /// <summary>
    /// Kinh nghiệm
    /// </summary>
    public int? Experience { get; set; }

    /// <summary>
    /// Note kinh nghiệm
    /// </summary>
    public string? ExperienceNote { get; set; }

    /// <summary>
    /// khả năng ngôn ngữ giao tiếp
    /// </summary>
    public int? LanguageAndCommunication { get; set; }

    /// <summary>
    /// Note ngôn ngữ giao tiếp
    /// </summary>
    public string? LanguageAndCommunicationNote { get; set; }

    /// <summary>
    /// Khả năng gắn bó
    /// </summary>
    public int? Motivation { get; set; }

    /// <summary>
    /// Note khả năng gắn bó
    /// </summary>
    public string? MotivationNote { get; set; }

    /// <summary>
    /// Nhận xét khác
    /// </summary>
    public string? OtherComments { get; set; }

    /// <summary>
    /// Đánh giá sau phỏng vấn (1: phù hợp, 2:Có thể phù hợp, 3:không phù hợp
    /// </summary>
    public int? ApplicantStatus { get; set; }

    /// <summary>
    /// Mức lương đề xuất
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// Trạng thái ký
    /// </summary>
    public bool? IsSign { get; set; }

    /// <summary>
    /// Ngày ký
    /// </summary>
    public DateTime? DateSign { get; set; }

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
    /// ID đơn ứng tuyển
    /// </summary>
    public int? HRRecruitmentCandidateID { get; set; }

    /// <summary>
    /// Tên người ký
    /// </summary>
    public string? SignName { get; set; }
}
