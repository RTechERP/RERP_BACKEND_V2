using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu trữ thông tin chi tiết về các câu hỏi trong bài kiểm tra tuyển dụng.
/// </summary>
public partial class HRRecruitmentQuestion
{
    /// <summary>
    /// ID duy nhất của câu hỏi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Số thứ tự của câu hỏi
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// Nội dung của câu hỏi
    /// </summary>
    public string? QuestionText { get; set; }

    /// <summary>
    /// Điểm số cho câu hỏi này
    /// </summary>
    public decimal? Point { get; set; }

    /// <summary>
    /// Loại câu hỏi (1: trắc nghiệm, 2: tự luận, điền vào chỗ trống)
    /// </summary>
    public int? QuestionType { get; set; }

    /// <summary>
    /// Cờ đánh dấu câu trả lời có phải là giá trị số không
    /// </summary>
    public bool? IsAnswerNumberValue { get; set; }

    /// <summary>
    /// Hướng dẫn cho câu hỏi tự luận - đáp án số
    /// </summary>
    public string? EssayGuidance { get; set; }

    /// <summary>
    /// ID của bài kiểm tra tuyển dụng mà câu hỏi này thuộc về
    /// </summary>
    public int? RecruitmentExamID { get; set; }

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
