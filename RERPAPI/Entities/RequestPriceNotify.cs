using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RequestPriceNotify
{
    public long ID { get; set; }

    public string? RequestPriceCode { get; set; }

    public int? RequestPriceID { get; set; }

    public int? UserID { get; set; }

    public bool? IsShow { get; set; }
}
