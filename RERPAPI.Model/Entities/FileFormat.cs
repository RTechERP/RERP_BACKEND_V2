using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Danh mục định dạng file được phép sử dụng trong hệ thống
/// </summary>
public partial class FileFormat
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Số thứ tự hiển thị
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// Tên định dạng file (PDF, Excel, Word...)
    /// </summary>
    public string? FormatName { get; set; }

    /// <summary>
    /// Phần mở rộng của file (.pdf, .xlsx, .docx...)
    /// </summary>
    public string? Extension { get; set; }

    /// <summary>
    /// Cờ đánh dấu bản ghi đã xóa mềm
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }
}
