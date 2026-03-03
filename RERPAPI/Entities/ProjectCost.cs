using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectCost
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? ListCostID { get; set; }

    public decimal? Money { get; set; }
}
