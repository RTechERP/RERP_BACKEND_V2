using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu điểm cộng trừ FiveS
/// </summary>
public partial class FiveSBonusMinu
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public int ID { get; set; }

    public int? FiveSRatingDetailID { get; set; }

    /// <summary>
    /// Điểm
    /// </summary>
    public decimal? Point { get; set; }

    /// <summary>
    /// Loại điểm 1.Cộng 2.Trừ
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Cờ xóa mềm (0: chưa xóa, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }

    public DateTime? DateMinus { get; set; }
}
