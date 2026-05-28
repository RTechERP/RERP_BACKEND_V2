using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Đánh giá chuyển hợp đồng New
/// </summary>
public partial class JobPerfomanceEvaluationNew
{
    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID người chuyển hợp đồng
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// ID Người đánh giá
    /// </summary>
    public int? EmployeeEvaluationID { get; set; }

    /// <summary>
    /// ID loại hợp đồng ( Loại Đánh giá )
    /// </summary>
    public int? EvaluationEmployeeLoaiHDID { get; set; }

    /// <summary>
    /// ID loại hợp đồng ( kết luận )
    /// </summary>
    public int? ConclusionEmployeeLoaiHDID { get; set; }

    /// <summary>
    /// TBP duyệt
    /// </summary>
    public int? TBPApproveID { get; set; }

    /// <summary>
    /// HCNS Duyệt
    /// </summary>
    public int? HCNSApproveID { get; set; }

    /// <summary>
    /// BGD Duyệt
    /// </summary>
    public int? BGDApproveID { get; set; }

    /// <summary>
    /// TBP duyệt
    /// </summary>
    public string? TBPApproveName { get; set; }

    /// <summary>
    /// HCNS Duyệt
    /// </summary>
    public string? HCNSApproveName { get; set; }

    /// <summary>
    /// BGD Duyệt
    /// </summary>
    public string? BGDApproveName { get; set; }

    /// <summary>
    /// Công việc chính
    /// 
    /// </summary>
    public string? MainJobmainResponsibilities { get; set; }

    /// <summary>
    /// A Năng lực chuyên môn
    /// </summary>
    public decimal? ProfessionalCompetency { get; set; }

    /// <summary>
    /// A Kiến thức chuyên môn nghiệp vụ
    /// </summary>
    public decimal? ProfessionalKnowledge { get; set; }

    /// <summary>
    /// A Kỹ năng sử dụng công cụ, hệ thống
    /// </summary>
    public decimal? ToolAndSystemSkills { get; set; }

    /// <summary>
    /// A Chất lượng công việc (độ chính xác, ít sai sót)
    /// </summary>
    public decimal? WorkQuality { get; set; }

    /// <summary>
    /// A Tiến độ &amp; khả năng đáp ứng công việc
    /// </summary>
    public decimal? WorkProgress { get; set; }

    /// <summary>
    /// A Khả năng xử lý tình huống
    /// </summary>
    public decimal? ProblemSolvingAbility { get; set; }

    /// <summary>
    /// B Tính chủ động trong công việc
    /// </summary>
    public decimal? Proactiveness { get; set; }

    /// <summary>
    /// B Khả năng phối hợp &amp; hỗ trợ phòng ban
    /// </summary>
    public decimal? CollaborationAndSupport { get; set; }

    /// <summary>
    /// B Kỹ năng giao tiếp &amp; làm việc nhóm
    /// </summary>
    public decimal? CommunicationAndTeamwork { get; set; }

    /// <summary>
    /// B Kết quả đầu ra công việc (Output/KPI chính)
    /// </summary>
    public decimal? WorkOutputKPI { get; set; }

    /// <summary>
    /// C Kỷ luật, tác phong &amp; thái độ
    /// </summary>
    public decimal? DisciplineAndAttitude { get; set; }

    /// <summary>
    /// C Tuân thủ nội quy, quy định Công ty &amp; Phòng
    /// </summary>
    public decimal? ComplianceWithRegulations { get; set; }

    /// <summary>
    /// C Chuyên cần (Đi làm đúng giờ, không nghỉ quá phép)
    /// </summary>
    public decimal? Attendance { get; set; }

    /// <summary>
    /// C Tác phong làm việc (Chỉn chu, chuyên nghiệp)
    /// </summary>
    public decimal? WorkStyle { get; set; }

    /// <summary>
    /// C Thái độ &amp; tinh thần trách nhiệm
    /// </summary>
    public decimal? AttitudeAndResponsibility { get; set; }

    /// <summary>
    /// D Mức độ phù hợp với văn hóa RTC
    /// </summary>
    public decimal? CulturalFitRTC { get; set; }

    /// <summary>
    /// D Tinh thần học hỏi &amp; cầu tiến
    /// </summary>
    public decimal? LearningAndGrowthMindset { get; set; }

    /// <summary>
    /// D Mức độ gắn bó với Công ty
    /// </summary>
    public decimal? CompanyCommitment { get; set; }

    /// <summary>
    /// Tổng điểm
    /// </summary>
    public decimal? TotalScore { get; set; }

    /// <summary>
    /// Xếp loại cuối cùng
    /// </summary>
    public string? EvaluationGrade { get; set; }

    /// <summary>
    /// Điểm mạnh
    /// </summary>
    public string? Strengths { get; set; }

    /// <summary>
    /// Điểm cần cải thiện
    /// </summary>
    public string? AreasForImprovement { get; set; }

    /// <summary>
    /// Kiến nghị/Khác
    /// </summary>
    public string? RecommendationsOrOther { get; set; }

    /// <summary>
    /// Kết luận khác
    /// </summary>
    public string? OtherConclusion { get; set; }

    /// <summary>
    /// Ngày đánh giá
    /// </summary>
    public DateTime? DateEvaluation { get; set; }

    /// <summary>
    /// Địa điểm đánh giá
    /// </summary>
    public string? LocationEvaluation { get; set; }

    /// <summary>
    /// Thời gian bắt đầu đánh giá
    /// </summary>
    public DateTime? DateStart { get; set; }

    /// <summary>
    /// Thời gian kết thúc đánh giá
    /// </summary>
    public DateTime? DateEnd { get; set; }

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
    /// A Năng lực chuyên môn
    /// </summary>
    public decimal? TBPProfessionalCompetency { get; set; }

    /// <summary>
    /// A Kiến thức chuyên môn nghiệp vụ
    /// </summary>
    public decimal? TBPProfessionalKnowledge { get; set; }

    /// <summary>
    /// A Kỹ năng sử dụng công cụ, hệ thống
    /// </summary>
    public decimal? TBPToolAndSystemSkills { get; set; }

    /// <summary>
    /// A Chất lượng công việc (độ chính xác, ít sai sót)
    /// </summary>
    public decimal? TBPWorkQuality { get; set; }

    /// <summary>
    /// A Tiến độ &amp; khả năng đáp ứng công việc
    /// </summary>
    public decimal? TBPWorkProgress { get; set; }

    /// <summary>
    /// A Khả năng xử lý tình huống
    /// </summary>
    public decimal? TBPProblemSolvingAbility { get; set; }

    /// <summary>
    /// B Tính chủ động trong công việc
    /// </summary>
    public decimal? TBPProactiveness { get; set; }

    /// <summary>
    /// B Khả năng phối hợp &amp; hỗ trợ phòng ban
    /// </summary>
    public decimal? TBPCollaborationAndSupport { get; set; }

    /// <summary>
    /// B Kỹ năng giao tiếp &amp; làm việc nhóm
    /// </summary>
    public decimal? TBPCommunicationAndTeamwork { get; set; }

    /// <summary>
    /// B Kết quả đầu ra công việc (Output/KPI chính)
    /// </summary>
    public decimal? TBPWorkOutputKPI { get; set; }

    /// <summary>
    /// C Kỷ luật, tác phong &amp; thái độ
    /// </summary>
    public decimal? TBPDisciplineAndAttitude { get; set; }

    /// <summary>
    /// C Tuân thủ nội quy, quy định Công ty &amp; Phòng
    /// </summary>
    public decimal? TBPComplianceWithRegulations { get; set; }

    /// <summary>
    /// C Chuyên cần (Đi làm đúng giờ, không nghỉ quá phép)
    /// </summary>
    public decimal? TBPAttendance { get; set; }

    /// <summary>
    /// C Tác phong làm việc (Chỉn chu, chuyên nghiệp)
    /// </summary>
    public decimal? TBPWorkStyle { get; set; }

    /// <summary>
    /// C Thái độ &amp; tinh thần trách nhiệm
    /// </summary>
    public decimal? TBPAttitudeAndResponsibility { get; set; }

    /// <summary>
    /// D Mức độ phù hợp với văn hóa RTC
    /// </summary>
    public decimal? TBPCulturalFitRTC { get; set; }

    /// <summary>
    /// D Tinh thần học hỏi &amp; cầu tiến
    /// </summary>
    public decimal? TBPLearningAndGrowthMindset { get; set; }

    /// <summary>
    /// D Mức độ gắn bó với Công ty
    /// </summary>
    public decimal? TBPCompanyCommitment { get; set; }

    /// <summary>
    /// Tổng điểm
    /// </summary>
    public decimal? TBPTotalScore { get; set; }

    /// <summary>
    /// Xếp loại cuối cùng
    /// </summary>
    public string? TBPEvaluationGrade { get; set; }

    /// <summary>
    /// TBP điểm mạnh
    /// </summary>
    public string? TBPStrengths { get; set; }

    /// <summary>
    /// TBP Điểm cần cải thiện
    /// </summary>
    public string? TBPAreasForImprovement { get; set; }

    /// <summary>
    /// TBP ID loại hợp đồng ( kết luận )
    /// </summary>
    public int? TBPConclusionEmployeeLoaiHDID { get; set; }

    /// <summary>
    /// TBP Kiến nghị/Khác
    /// </summary>
    public string? TBPRecommendationsOrOther { get; set; }
}
