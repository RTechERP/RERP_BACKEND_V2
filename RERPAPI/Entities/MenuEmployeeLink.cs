using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class MenuEmployeeLink
{
    public int ID { get; set; }

    public int? MenuID { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UserGroupID { get; set; }
}
