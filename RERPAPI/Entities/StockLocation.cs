using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class StockLocation
{
    public int ID { get; set; }

    public string? StockLocationCode { get; set; }

    public string? StockLocationName { get; set; }

    public int? StockCode { get; set; }
}
