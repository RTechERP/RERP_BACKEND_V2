using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPIDetailUser
{
    public int ID { get; set; }

    public int? KPIID { get; set; }

    public decimal? PercentKPI { get; set; }

    public int? UserID { get; set; }

    public int? Quy { get; set; }
}
