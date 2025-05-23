﻿using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProductGroupWarehouse
{
    public int ID { get; set; }

    public int? ProductGroupID { get; set; }

    public int? WarehouseID { get; set; }

    public int? EmployeeID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
