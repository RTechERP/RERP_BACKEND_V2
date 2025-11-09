using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class QuotationNCCDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? QuotationNCCID { get; set; }

    public int? ProductID { get; set; }

    public int? Qty { get; set; }

    public int? UnitPrice { get; set; }

    public int? IntoMoney { get; set; }

    public string? IntendTime { get; set; }
}
