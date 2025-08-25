using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TSAssetRecoveryDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? TSAssetRecoveryID { get; set; }

    public int? AssetManagementID { get; set; }

    public int? Quantity { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public int? LastTSStatusAssetID { get; set; }

    public int? LastEmployeeID { get; set; }

    public bool? IsDeleted { get; set; }
}
