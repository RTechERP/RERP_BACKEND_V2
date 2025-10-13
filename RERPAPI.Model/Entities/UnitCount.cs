using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class UnitCount
{
    public int ID { get; set; }

    public string? UnitCode { get; set; }

    public string? UnitName { get; set; }

    public bool isDeleted { get; set; }
}
