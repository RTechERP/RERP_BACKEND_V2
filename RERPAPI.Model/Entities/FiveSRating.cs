using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu thông tin đánh giá FiveS theo tháng/năm
/// </summary>
public partial class FiveSRating
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Năm đánh giá
    /// </summary>
    public int? YearValue { get; set; }

    /// <summary>
    /// Tháng đánh giá
    /// </summary>
    public int? MonthValue { get; set; }

    /// <summary>
    /// Ngày thực hiện đánh giá
    /// </summary>
    public DateTime? RatingDate { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Mã đánh giá (tự sinh: Năm_Tháng_STT)
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Đánh dấu đã xóa (0: chưa xóa, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }
}
