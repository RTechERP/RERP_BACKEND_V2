using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng danh mục quy tắc lỗi 5S
/// </summary>
public partial class FiveSRuleError
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Tên lỗi / quy tắc lỗi 5S
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Điểm đánh giá tổng (legacy hoặc dùng chung)
    /// </summary>
    public decimal? Point { get; set; }

    /// <summary>
    /// Ghi chú bổ sung
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Mô tả chi tiết quy tắc lỗi
    /// </summary>
    public string? Description { get; set; }

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

    /// <summary>
    /// Điểm trừ
    /// </summary>
    public decimal? MinusPoint { get; set; }

    /// <summary>
    /// Điểm cộng
    /// </summary>
    public decimal? BonusPoint { get; set; }

    /// <summary>
    /// Loại điểm (1: Điểm cộng, 2: Điểm trừ)
    /// </summary>
    public int? TypePoint { get; set; }

    /// <summary>
    /// FK tới bảng FiveSError
    /// </summary>
    public int? FiveSErrorID { get; set; }

    /// <summary>
    /// Mức đánh giá (A/B/C hoặc JSON config)
    /// </summary>
    public string? RatingLevels { get; set; }
}
