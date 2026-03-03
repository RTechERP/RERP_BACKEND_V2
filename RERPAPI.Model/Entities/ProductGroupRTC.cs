using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProductGroupRTC
{
    public int ID { get; set; }

    /// <summary>
    /// Mã nhóm thiết bị
    /// </summary>
    public string? ProductGroupNo { get; set; }

    /// <summary>
    /// Tên nhóm thiết bị
    /// </summary>
    public string? ProductGroupName { get; set; }

    public int? NumberOrder { get; set; }

    public int? WarehouseID { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    /// 1: Kho demo; 2: Kho AGV
    /// </summary>
    public int? WarehouseType { get; set; }
}
