using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu trữ thông tin về hình ảnh liên quan đến các câu hỏi tuyển dụng.
/// </summary>
public partial class HRRecruitmentQuestionImage
{
    /// <summary>
    /// ID duy nhất của hình ảnh câu hỏi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID của câu hỏi mà hình ảnh này thuộc về
    /// </summary>
    public int? RecruitmentQuestionID { get; set; }

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

    /// <summary>
    /// Ngày cập nhật bản ghi gần nhất
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Cờ đánh dấu bản ghi đã bị xóa mềm (0: không xóa, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }
}
