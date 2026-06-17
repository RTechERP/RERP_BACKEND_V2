using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu trữ thông tin về hình ảnh liên quan đến kết quả chi tiết bài kiểm tra tuyển dụng (ví dụ: ảnh bài làm tự luận).
/// </summary>
public partial class HRRecruitmentExamResultImage
{
    /// <summary>
    /// ID duy nhất của hình ảnh kết quả bài kiểm tra
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID của chi tiết kết quả bài kiểm tra mà hình ảnh này thuộc về
    /// </summary>
    public int? RecruitmentExamResultDetailID { get; set; }

    /// <summary>
    /// Tên gốc của file hình ảnh
    /// </summary>
    public string? FileNameOrigin { get; set; }

    /// <summary>
    /// Phần mở rộng của file hình ảnh
    /// </summary>
    public string? Extension { get; set; }

    /// <summary>
    /// Đường dẫn gốc của hình ảnh
    /// </summary>
    public string? OriginPath { get; set; }

    /// <summary>
    /// Đường dẫn lưu trữ hình ảnh trên server
    /// </summary>
    public string? ServerPath { get; set; }

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

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Cờ đánh dấu bản ghi đã bị xóa mềm (0: không xóa, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }
}
