using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectTaskDifficulty
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Mức độ của độ khó
    /// </summary>
    public int? Level { get; set; }

    /// <summary>
    /// Tên độ khó
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Yếu tố 
    /// </summary>
    public decimal? Factor { get; set; }

    /// <summary>
    /// Mô tả độ khó
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gợi ý đánh giá độ khó cho công việc
    /// </summary>
    public string? SuggestedReview { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật bản ghi
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Ngươì cập nhật bản ghi
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái khóa mềm 
    /// </summary>
    public bool? IsDeleted { get; set; }
}
