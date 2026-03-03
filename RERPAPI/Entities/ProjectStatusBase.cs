using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectStatusBase
{
    public int ID { get; set; }

    public string? ProjectStatusCode { get; set; }

    public string? ProjectStatusName { get; set; }
}
