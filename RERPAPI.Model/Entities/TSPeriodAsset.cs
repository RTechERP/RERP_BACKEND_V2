using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Khoa
/// </summary>
public partial class TSPeriodAsset
{
    public int ID { get; set; }

    public int? AssetManagementID { get; set; }

    /// <summary>
    /// Chu kỳ bảo dưỡng
    /// </summary>
    public int? PeriodMaintenance { get; set; }

    /// <summary>
    /// Ngày bảo dưỡng gần nhất
    /// </summary>
    public DateTime? DateMaintenanceNearest { get; set; }

    /// <summary>
    /// Ngày bảo dưỡng tiếp theo
    /// </summary>
    public DateTime? DateMaintenanceNext { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
