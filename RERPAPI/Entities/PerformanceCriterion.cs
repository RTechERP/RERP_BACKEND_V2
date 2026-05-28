using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Tiêu chí đánh giá
/// </summary>
public partial class PerformanceCriterion
{
    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Mã tiêu chí đánh giá
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Tên tiêu chí đánh giá bằng tiếng việt
    /// </summary>
    public string? NameVI { get; set; }

    public string? SubTitleVI { get; set; }

    /// <summary>
    /// Mô tả tiếng việt
    /// </summary>
    public string? DescriptionVI { get; set; }

    /// <summary>
    /// Tên tiêu chí bằng tiếng anh
    /// </summary>
    public string? NameEN { get; set; }

    public string? SubTitleEN { get; set; }

    /// <summary>
    /// Mô tả tiếng anh
    /// </summary>
    public string? DescriptionEN { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người update
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày update
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Trạng thái xoá
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// STT
    /// </summary>
    public int? STT { get; set; }

    public bool? IsPublish { get; set; }
}
