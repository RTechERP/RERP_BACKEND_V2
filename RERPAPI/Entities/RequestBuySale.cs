using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RequestBuySale
{
    public int ID { get; set; }

    public int? Status { get; set; }

    public string? RequestCode { get; set; }

    public int? UserID { get; set; }

    public string? Project { get; set; }

    public string? POCode { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public bool? IsApproved { get; set; }

    public int? CustomerID { get; set; }
}
