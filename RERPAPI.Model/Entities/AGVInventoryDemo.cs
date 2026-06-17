using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AGVInventoryDemo
{
    public int ID { get; set; }

    public int? AGVProductID { get; set; }

    public int? WarehouseID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
