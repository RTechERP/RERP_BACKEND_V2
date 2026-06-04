using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ListCost
{
    public int ID { get; set; }

    public string? CostCode { get; set; }

    public string? CostName { get; set; }
}
