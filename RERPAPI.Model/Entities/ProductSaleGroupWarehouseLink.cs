using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProductSaleGroupWarehouseLink
{
    public int ID { get; set; }

    /// <summary>
    /// Link bảng ProductSale
    /// </summary>
    public int? ProductSaleID { get; set; }

    /// <summary>
    /// Link bảng ProductGroupWarehouse
    /// </summary>
    public int? ProductGroupWarehouseID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
