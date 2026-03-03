using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class AccountingContractLog
{
    public int ID { get; set; }

    public int? AccountingContractID { get; set; }

    public DateTime? DateLog { get; set; }

    public bool? IsReceivedContract { get; set; }

    public string? ContentLog { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsApproved { get; set; }
}
