using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProductGroup
{
    public int ID { get; set; }

    public string? ProductGroupID { get; set; }

    public string? ProductGroupName { get; set; }

    public bool? IsVisible { get; set; }

    public int? EmployeeID { get; set; }
}
