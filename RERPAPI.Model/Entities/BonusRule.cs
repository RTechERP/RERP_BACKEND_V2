using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BonusRule
{
    public int ID { get; set; }

    public decimal? Min { get; set; }

    public decimal? Max { get; set; }

    public decimal? Value { get; set; }

    public decimal? PercentRule { get; set; }

    public int? GroupSaleID { get; set; }

    public int? SaleUserTypeID { get; set; }

    /// <summary>
    /// 0: dấu &lt;, 1: dấu &lt;=
    /// </summary>
    public int? CompareMAX { get; set; }

    public int? CompareMIN { get; set; }

    public int? Quy { get; set; }

    public int? Year { get; set; }
}
