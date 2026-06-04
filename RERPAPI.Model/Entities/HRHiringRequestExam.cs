using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng liên kết các yêu cầu tuyển dụng với các bài kiểm tra tuyển dụng.
/// </summary>
public partial class HRHiringRequestExam
{
    /// <summary>
    /// ID duy nhất của liên kết
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID của yêu cầu tuyển dụng
    /// </summary>
    public int HiringRequestID { get; set; }

    /// <summary>
    /// ID của bài kiểm tra tuyển dụng
    /// </summary>
    public int RecruitmentExamID { get; set; }

    /// <summary>
    /// Cờ đánh dấu bài kiểm tra có đang hoạt động cho yêu cầu tuyển dụng này không
    /// </summary>
    public bool? IsActive { get; set; }

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
