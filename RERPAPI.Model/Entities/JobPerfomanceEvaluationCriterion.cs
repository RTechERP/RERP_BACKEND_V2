using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Tiêu chí đánh giá chuyển hợp đồng
/// </summary>
public partial class JobPerfomanceEvaluationCriterion
{
    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID đánh giá chuyển hợp đồng
    /// </summary>
    public int? JobPerfomanceEvaluationID { get; set; }

    /// <summary>
    /// STT
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// Mã tiêu chí
    /// </summary>
    public string? CodeCriteria { get; set; }

    /// <summary>
    /// Tên tiêu chí
    /// </summary>
    public string? NameCriteria { get; set; }

    /// <summary>
    /// Kết quả đánh giá
    /// </summary>
    public int? ResultEvaluation { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

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
}
