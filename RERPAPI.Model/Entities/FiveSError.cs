using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu thông tin lỗi 5S
/// </summary>
public partial class FiveSError
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Số thứ tự hiển thị
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// Loại lỗi (1:S1 - Seiri, 2:S2 - Seiton, 3:S3 - Seiso, 4:S4 - Seiketsu, 5:S5 - Shitsuke)
    /// </summary>
    public int? TypeError { get; set; }

    /// <summary>
    /// Chi tiết nội dung lỗi
    /// </summary>
    public string? DetailError { get; set; }

    /// <summary>
    /// Ngày cập nhật dữ liệu
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Cờ xóa mềm (0: chưa xóa, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }
}
