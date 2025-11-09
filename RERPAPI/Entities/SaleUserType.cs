using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class SaleUserType
{
    public int ID { get; set; }

    public string? SaleUserTypeCode { get; set; }

    public string? SaleUserTypeName { get; set; }

    public decimal? PercentBonus { get; set; }
}
