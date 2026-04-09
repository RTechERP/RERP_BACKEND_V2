using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Duyệt đánh giá chuyển hợp đồng
/// </summary>
public partial class JobPerfomanceEvaluationApprove
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
    /// 1:Người lao động; 2:TBP ; 3:Phòng HR ; 4:BGD
    /// </summary>
    public int? Step { get; set; }

    /// <summary>
    /// Bước duyệt
    /// </summary>
    public string? StepName { get; set; }

    /// <summary>
    /// Ngày duyệt/Huỷ duyệt
    /// </summary>
    public DateTime? DateApproved { get; set; }

    /// <summary>
    /// Lý do huỷ duyệt
    /// </summary>
    public string? ReasonUnApproved { get; set; }

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
