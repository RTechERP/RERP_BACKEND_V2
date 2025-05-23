using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Warehouse
{
    public int ID { get; set; }

    public string? WarehouseCode { get; set; }

    public string? WarehouseName { get; set; }
}
