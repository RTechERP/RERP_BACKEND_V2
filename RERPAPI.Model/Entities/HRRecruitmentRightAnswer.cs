using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu trữ các câu trả lời đúng cho các câu hỏi tuyển dụng.
/// </summary>
public partial class HRRecruitmentRightAnswer
{
    /// <summary>
    /// ID duy nhất của bản ghi câu trả lời đúng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID của câu hỏi
    /// </summary>
    public int? RecruitmentQuestionID { get; set; }

    /// <summary>
    /// ID của câu trả lời đúng
    /// </summary>
    public int? RecruitmentAnswerID { get; set; }

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
    /// Ngày cập nhật bản ghi 
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
