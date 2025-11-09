using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RequestInvoiceStatusLink
{
    public int ID { get; set; }

    public int? RequestInvoiceID { get; set; }

    public int? StatusID { get; set; }

    public int? IsApproved { get; set; }

    public bool? IsCurrent { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? AmendReason { get; set; }
}
