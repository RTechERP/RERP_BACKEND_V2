using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Stock
{
    public int ID { get; set; }

    public string? StockCode { get; set; }

    public string? StockName { get; set; }

    public string? PhoneNumber { get; set; }

    public int? StockManager { get; set; }
}
