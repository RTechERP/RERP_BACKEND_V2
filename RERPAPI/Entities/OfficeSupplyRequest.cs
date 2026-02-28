using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class OfficeSupplyRequest
{
    public int ID { get; set; }

    public int? UserID { get; set; }

    public int? OfficeSupplyID { get; set; }

    public int? Quantity { get; set; }

    public int? QuantityReceived { get; set; }

    public string? Note { get; set; }

    public int? UserIDReceive { get; set; }

    public DateTime? DateRequest { get; set; }

    public bool? ExceedsLimit { get; set; }

    public string? Reason { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsApproved { get; set; }

    public int? ApprovedID { get; set; }

    public DateTime? DateApproved { get; set; }
}
