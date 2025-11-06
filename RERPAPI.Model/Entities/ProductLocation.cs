using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProductLocation
{
    public int ID { get; set; }

    public string? LocationCode { get; set; }

    public string? OldLocationName { get; set; }

    public string? LocationName { get; set; }

    public int? WarehouseID { get; set; }

    public decimal? CoordinatesX { get; set; }

    public decimal? CoordinatesY { get; set; }

    public int? STT { get; set; }

    /// <summary>
    /// 1: Tủ mũ &amp; quần áo; 2: Tủ giày
    /// </summary>
    public int? LocationType { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
