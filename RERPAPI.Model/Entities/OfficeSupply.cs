using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class OfficeSupply
{
    public int ID { get; set; }

    public string? CodeRTC { get; set; }

    public string? CodeNCC { get; set; }

    public string? NameRTC { get; set; }

    public string? NameNCC { get; set; }

    public int? SupplyUnitID { get; set; }

    public decimal? Price { get; set; }

    public int? RequestLimit { get; set; }

    public int? Type { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
