using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PaymentOrderPO
{
    public int ID { get; set; }

    public int? PaymentOrderID { get; set; }

    public int? POKHID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public int? POKHDetailID { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
