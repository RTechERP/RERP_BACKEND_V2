using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class GroupSalesUser
{
    public int ID { get; set; }

    public int? GroupSalesID { get; set; }

    public int? UserID { get; set; }

    public int? SaleUserTypeID { get; set; }

    public int? LeaderID { get; set; }

    public int? ParentID { get; set; }

    public string? Note { get; set; }
}
