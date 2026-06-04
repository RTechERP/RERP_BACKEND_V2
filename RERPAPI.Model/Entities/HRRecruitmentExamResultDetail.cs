using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu trữ chi tiết kết quả của từng câu hỏi trong một bài kiểm tra tuyển dụng của một ứng viên.
/// </summary>
public partial class HRRecruitmentExamResultDetail
{
    /// <summary>
    /// ID duy nhất của chi tiết kết quả
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID của câu hỏi
    /// </summary>
    public int? RecruitmentQuestionID { get; set; }

    /// <summary>
    /// ID của câu trả lời mà ứng viên đã chọn (nếu là trắc nghiệm)
    /// </summary>
    public int? RecruitmentAnswerID { get; set; }

    /// <summary>
    /// ID của kết quả bài kiểm tra tổng thể
    /// </summary>
    public int? RecruitmentExamResultID { get; set; }

    /// <summary>
    /// Nội dung câu trả lời của ứng viên (nếu là tự luận)
    /// </summary>
    public string? AnswerText { get; set; }

    /// <summary>
    /// Điểm đạt được cho câu hỏi này
    /// </summary>
    public decimal? Score { get; set; }

    /// <summary>
    /// Cờ đánh dấu câu hỏi đã được chấm điểm chưa (0: chưa, 1: rồi)
    /// </summary>
    public bool? IsGraded { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người cập nhật bản ghi gần nhất
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật bản ghi gần nhất
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Cờ đánh dấu bản ghi đã bị xóa mềm (0: không xóa, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }
}
