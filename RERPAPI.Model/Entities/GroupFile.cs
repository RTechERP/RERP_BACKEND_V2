using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class GroupFile
{
    public int ID { get; set; }

    public string? GroupFileCode { get; set; }

    public string? GroupFileName { get; set; }
}
