using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectTypeBase
{
    public int ID { get; set; }

    public string? ProjectTypeCode { get; set; }

    public string? ProjectTypeName { get; set; }
}
