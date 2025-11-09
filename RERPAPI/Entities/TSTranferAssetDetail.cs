using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class TSTranferAssetDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? AssetManagementID { get; set; }

    public int? TSTranferAssetID { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Note { get; set; }

    public int? Quantity { get; set; }
}
