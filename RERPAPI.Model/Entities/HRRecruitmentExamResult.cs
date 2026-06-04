using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu trữ kết quả tổng thể của một bài kiểm tra tuyển dụng của một ứng viên.
/// </summary>
public partial class HRRecruitmentExamResult
{
    /// <summary>
    /// ID duy nhất của kết quả bài kiểm tra
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID của bài kiểm tra tuyển dụng
    /// </summary>
    public int? RecruitmentExamID { get; set; }

    /// <summary>
    /// ID của ứng viên/nhân viên làm bài kiểm tra
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Tổng số câu trả lời đúng
    /// </summary>
    public int? TotalCorrect { get; set; }

    /// <summary>
    /// Tổng số câu trả lời sai
    /// </summary>
    public int? TotalIncorrect { get; set; }

    /// <summary>
    /// Tổng điểm đạt được
    /// </summary>
    public decimal? TotalScore { get; set; }

    /// <summary>
    /// Điểm tối đa có thể đạt được
    /// </summary>
    public decimal? MaxPossibleScore { get; set; }

    /// <summary>
    /// Tỷ lệ phần trăm câu trả lời đúng
    /// </summary>
    public decimal? PercentageCorrect { get; set; }

    /// <summary>
    /// Trạng thái kết quả (0: đang làm, 1: đã nộp bài chờ chấm, 2: đã có điểm)
    /// </summary>
    public int? StatusResult { get; set; }

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

    public int? RemainingSeconds { get; set; }

    public int? HiringRequestID { get; set; }
}
