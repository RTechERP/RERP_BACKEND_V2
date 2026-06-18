using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ESLConfig
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Dùng để gọi Config lấy giá trị (tên config)
    /// </summary>
    public string ConfigKey { get; set; } = null!;

    /// <summary>
    /// Giá trị của config
    /// </summary>
    public string ConfigValue { get; set; } = null!;

    /// <summary>
    /// Mô tả config
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa
    /// </summary>
    public bool? IsDeleted { get; set; }
}
