using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Khoa
/// </summary>
public partial class TSRepairAsset
{
    public int ID { get; set; }

    public int? AssetManagementID { get; set; }

    /// <summary>
    /// Ngày sửa
    /// </summary>
    public DateTime? DateRepair { get; set; }

    public DateTime? DateEndRepair { get; set; }

    /// <summary>
    /// Ngày đưa vào sử dụng lại
    /// </summary>
    public DateTime? DateReuse { get; set; }

    /// <summary>
    /// Tên đơn vị sửa chữa, bảo dưỡng
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Chi phí dự kiến
    /// </summary>
    public decimal? ExpectedCost { get; set; }

    public decimal? ActualCosts { get; set; }

    public string? ContentRepair { get; set; }

    /// <summary>
    /// Lý do sửa
    /// </summary>
    public string? Reason { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
