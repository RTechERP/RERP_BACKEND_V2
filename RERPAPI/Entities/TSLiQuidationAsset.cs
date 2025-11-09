using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class TSLiQuidationAsset
{
    public int ID { get; set; }

    public int? AssetManagementID { get; set; }

    /// <summary>
    /// Người phê duyệt thanh lý tài sản
    /// </summary>
    public int? EmployeeID { get; set; }

    public bool? IsApproved { get; set; }

    /// <summary>
    /// Ngày đề nghị thanh lý
    /// </summary>
    public DateTime? DateSuggest { get; set; }

    public DateTime? DateLiquidation { get; set; }

    public string? Reason { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
