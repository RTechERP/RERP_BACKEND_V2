using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Khoa
/// </summary>
public partial class TSLostReportAsset
{
    public int ID { get; set; }

    public int? AssetManagementID { get; set; }

    /// <summary>
    /// Ngày báo mất tài sản
    /// </summary>
    public DateTime? DateLostReport { get; set; }

    /// <summary>
    /// Lý do báo mất tài sản
    /// </summary>
    public string? Reason { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
