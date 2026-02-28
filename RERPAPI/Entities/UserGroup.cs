using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class UserGroup
{
    public int ID { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }

    public int? Leader { get; set; }

    public int? DepartmentID { get; set; }
}
