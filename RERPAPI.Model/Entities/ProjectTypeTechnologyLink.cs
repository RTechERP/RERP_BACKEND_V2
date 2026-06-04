using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectTypeTechnologyLink
{
    /// <summary>
    /// Khóa chính, tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID liên kết loại dự án (ProjectTypeLink)
    /// </summary>
    public int? ProjectTypeLinkID { get; set; }

    /// <summary>
    /// ID công nghệ sử dụng
    /// </summary>
    public int? TechnologyID { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Cờ xóa mềm (0: hoạt động, 1: đã xóa)
    /// </summary>
    public bool? IsDeleted { get; set; }
}
