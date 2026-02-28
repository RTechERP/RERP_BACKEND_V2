using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class HandoverWarehouseAsset
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? HandoverID { get; set; }

    public int? EmployeeID { get; set; }

    public string? SignedBy { get; set; }

    public string? ProductName { get; set; }

    public string? ProductGroupName { get; set; }

    public int? BorrowQty { get; set; }

    public string? Unit { get; set; }

    public string? Status { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? BorrowID { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsSigned { get; set; }
}
