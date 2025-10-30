using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AddressStock
{
    public int ID { get; set; }

    public string? Address { get; set; }

    public int CustomerID { get; set; }
    public bool? IsDeleted { get; set; }
}
