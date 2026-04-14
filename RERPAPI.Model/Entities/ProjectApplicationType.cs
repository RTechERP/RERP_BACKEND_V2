using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectApplicationType
{
    /// <summary>
    /// Khóa chính, tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Tên kiểu ứng dụng
    /// </summary>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// Mô tả kiểu ứng dụng
    /// </summary>
    public string? Descriptions { get; set; }

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

    public int? ProjectTypeID { get; set; }
}
