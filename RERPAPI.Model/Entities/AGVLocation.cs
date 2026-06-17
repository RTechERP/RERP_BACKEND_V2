using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AGVLocation
{
    public int ID { get; set; }

    public string? AGVLocationCode { get; set; }

    public string? AGVLocationName { get; set; }

    public int? AGVProductGroupID { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? WarehouseID { get; set; }
}
