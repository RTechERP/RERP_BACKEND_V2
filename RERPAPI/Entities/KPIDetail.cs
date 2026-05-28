using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPIDetail
{
    public int ID { get; set; }

    public string? KPI { get; set; }

    public string? Note { get; set; }

    public int? GroupSalesID { get; set; }
}
