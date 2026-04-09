using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Đánh giá chuyển hợp đồng
/// </summary>
public partial class JobPerfomanceEvaluation
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
    /// ID Cán bộ quản lý
    /// </summary>
    public int? LeaderID { get; set; }

    /// <summary>
    /// ID Người đánh giá
    /// </summary>
    public int? EmployeeEvaluationID { get; set; }

    /// <summary>
    /// Thời gian bắt đầu đánh giá
    /// </summary>
    public DateTime? DateStart { get; set; }

    /// <summary>
    /// Thời gian kết thúc đánh giá
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Năng lực chuyên môn
    /// </summary>
    public string? ProfessionalCompetence { get; set; }

    /// <summary>
    /// Mức độ hoành thành công việc được giao
    /// </summary>
    public string? JobComplete { get; set; }

    /// <summary>
    /// Ý thức, tinh thần trách nhiệm
    /// </summary>
    public string? Consciousness { get; set; }

    /// <summary>
    /// Chấp hành nội quy
    /// </summary>
    public string? Regulations { get; set; }

    /// <summary>
    /// Khả năng khác
    /// </summary>
    public string? OtherPossiblities { get; set; }

    /// <summary>
    /// Ý kiến, nguyện vọng cá nhân
    /// </summary>
    public string? PesonalWishes { get; set; }

    /// <summary>
    /// Công việc được giao
    /// </summary>
    public string? AssignTask { get; set; }

    /// <summary>
    /// Kết quả thực hiện
    /// </summary>
    public string? ResultOfImplementation { get; set; }

    /// <summary>
    /// Nhận xét khác
    /// </summary>
    public string? OtherComment { get; set; }

    /// <summary>
    /// Kiến nghị
    /// </summary>
    public string? Recomment { get; set; }

    /// <summary>
    /// ID loại hợp đồng ( kết luận )
    /// </summary>
    public int? EmployeeLoaiHDID { get; set; }

    /// <summary>
    /// Mức lương đề xuất
    /// </summary>
    public decimal? SalaryPropose { get; set; }

    /// <summary>
    /// Ngày đánh giá
    /// </summary>
    public DateTime? DateEvaluation { get; set; }

    /// <summary>
    /// Địa điểm đánh giá
    /// </summary>
    public string? LocationEvaluation { get; set; }

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
}
