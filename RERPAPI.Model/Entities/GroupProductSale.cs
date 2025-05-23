using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class GroupProductSale
{
    public int ID { get; set; }

    public int? ProductGroupID { get; set; }

    public string? GroupName { get; set; }
}
