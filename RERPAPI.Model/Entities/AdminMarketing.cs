using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AdminMarketing
{
    public int ID { get; set; }

    public string? KPI { get; set; }

    public decimal? CompletionRate { get; set; }

    public int? Quantity { get; set; }

    public int? Item { get; set; }
}
