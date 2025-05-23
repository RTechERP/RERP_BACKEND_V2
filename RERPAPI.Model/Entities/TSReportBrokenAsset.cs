using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TSReportBrokenAsset
{
    public int ID { get; set; }

    public int? AssetManagementID { get; set; }

    /// <summary>
    /// Ngày báo hỏng tài sản
    /// </summary>
    public DateTime? DateReportBroken { get; set; }

    public string? Reason { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
