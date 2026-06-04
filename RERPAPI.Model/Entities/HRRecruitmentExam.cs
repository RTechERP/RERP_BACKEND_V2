using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu trữ thông tin chi tiết về các bài kiểm tra tuyển dụng.
/// </summary>
public partial class HRRecruitmentExam
{
    /// <summary>
    /// ID duy nhất của bài kiểm tra
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Tên của bài kiểm tra
    /// </summary>
    public string? NameExam { get; set; }

    /// <summary>
    /// Mã của bài kiểm tra
    /// </summary>
    public string? CodeExam { get; set; }

    /// <summary>
    /// Điểm đạt của bài kiểm tra
    /// </summary>
    public decimal? Goal { get; set; }

    /// <summary>
    /// Thời gian làm bài kiểm tra
    /// </summary>
    public decimal? TestTime { get; set; }

    /// <summary>
    /// Loại bài kiểm tra (1: trắc nghiệm, 2: tự luận, 3: trắc nghiệm &amp; tự luận)
    /// </summary>
    public int? ExamType { get; set; }

    /// <summary>
    /// ID của phòng ban
    /// </summary>
    public int? DepartmentID { get; set; }

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
