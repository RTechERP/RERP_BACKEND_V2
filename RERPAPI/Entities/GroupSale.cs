using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class GroupSale
{
    public int ID { get; set; }

    public string? GroupSalesCode { get; set; }

    public string? GroupSalesName { get; set; }

    public string? MainIndexID { get; set; }
}
