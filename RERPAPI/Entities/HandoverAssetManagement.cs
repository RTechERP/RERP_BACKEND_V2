using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class HandoverAssetManagement
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? HandoverID { get; set; }

    public string? TSAssetCode { get; set; }

    public string? TSAssetName { get; set; }

    public int? Quantity { get; set; }

    public string? Unit { get; set; }

    public string? Status { get; set; }

    public int? EmployeeID { get; set; }

    public string? SignedBy { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsSigned { get; set; }
}
