using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectTaskWork
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID của Công việc (ProjectItem)
    /// </summary>
    public int? ProjectTaskID { get; set; }

    /// <summary>
    /// Ngày làm việc
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Địa điểm làm việc
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Có làm việc hay không
    /// </summary>
    public bool? IsWork { get; set; }

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

    /// <summary>
    /// Thời gian dự kiến (h)
    /// </summary>
    public decimal? EstimatedTime { get; set; }
}
