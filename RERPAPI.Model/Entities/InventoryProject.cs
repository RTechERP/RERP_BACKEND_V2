using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class InventoryProject
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? ProductSaleID { get; set; }

    /// <summary>
    /// Người giữ
    /// </summary>
    public int? EmployeeID { get; set; }

    public int? WarehouseID { get; set; }

    public decimal? Quantity { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
