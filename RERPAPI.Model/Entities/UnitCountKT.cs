using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class UnitCountKT
{
    public int ID { get; set; }

    public string? UnitCountCode { get; set; }

    public string? UnitCountName { get; set; }
    public bool? IsDeleted { get; set; }
}
