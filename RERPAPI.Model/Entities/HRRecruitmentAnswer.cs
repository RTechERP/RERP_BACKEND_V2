using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu trữ các lựa chọn trả lời cho các câu hỏi tuyển dụng.
/// </summary>
public partial class HRRecruitmentAnswer
{
    /// <summary>
    /// ID duy nhất của câu trả lời
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Nội dung của câu trả lời
    /// </summary>
    public string? AnswersText { get; set; }

    /// <summary>
    /// Giá trị số của câu trả lời (nếu có)
    /// </summary>
    public int? AnswersNumber { get; set; }

    /// <summary>
    /// Loại câu hỏi mà câu trả lời này thuộc về (1:trắc nghiệm, 2: tự luận)
    /// </summary>
    public int? QuestionType { get; set; }

    /// <summary>
    /// Đường dẫn đến hình ảnh liên quan đến câu trả lời
    /// </summary>
    public string? ImageLink { get; set; }

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
    /// ID của câu hỏi mà câu trả lời này thuộc về
    /// </summary>
    public int? RecruitmentQuestionID { get; set; }

    /// <summary>
    /// Cờ đánh dấu bản ghi đã bị xóa mềm (0: không xóa, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }
}
