using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class OfficeSupplyRequestsDetail
{
    public int ID { get; set; }

    public int? OfficeSupplyRequestsID { get; set; }

    public int? EmployeeID { get; set; }

    public int? OfficeSupplyID { get; set; }

    public int? Quantity { get; set; }

    public int? QuantityReceived { get; set; }

    public bool? ExceedsLimit { get; set; }

    public string? Reason { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
