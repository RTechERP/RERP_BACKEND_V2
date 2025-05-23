using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PriceCheck
{
    public int ID { get; set; }

    public string? ProductName { get; set; }

    public int? ProductID { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? LeadTime { get; set; }

    public string? Note { get; set; }

    public int? SupplierID { get; set; }

    public int? STT { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBY { get; set; }

    public string? ProductCode { get; set; }

    public int? RequestID { get; set; }

    public decimal? Qty { get; set; }

    public bool? IsSelected { get; set; }
}
