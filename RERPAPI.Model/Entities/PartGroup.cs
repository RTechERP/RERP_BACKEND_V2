using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PartGroup
{
    public int ID { get; set; }

    public string? PartGroupCode { get; set; }

    public string? PartGroupName { get; set; }

    public string? Note { get; set; }
}
