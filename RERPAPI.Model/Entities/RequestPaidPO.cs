using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class RequestPaidPO
{
    public int ID { get; set; }

    public int? PurchaseOrderID { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? DatePaidExpected { get; set; }

    public DateTime? DatePaid { get; set; }

    public int? RequestPaidPOStatus { get; set; }
}
