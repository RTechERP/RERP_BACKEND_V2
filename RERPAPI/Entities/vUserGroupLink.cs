using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class vUserGroupLink
{
    public int ID { get; set; }

    public int? UserGroupID { get; set; }

    public int? UserID { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int? DepartmentID { get; set; }

    public string? FullName { get; set; }

    public string? UserCode { get; set; }
}
