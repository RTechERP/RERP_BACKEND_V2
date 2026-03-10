using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class InventoryDemo
{
    /// <summary>
    /// ID tồn kho demo
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID sản phẩm kho demo
    /// </summary>
    public int? ProductRTCID { get; set; }

    /// <summary>
    /// ID kho
    /// </summary>
    public int? WarehouseID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? AGVProductID { get; set; }
}
