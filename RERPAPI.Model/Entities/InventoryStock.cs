using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class InventoryStock
{
    public int ID { get; set; }

    public int? InventoryID { get; set; }

    public decimal? Quantity { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? EmployeeStock { get; set; }

    public int? ProductSaleID { get; set; }

    public int? WarehouseID { get; set; }

    public int? ProjectTypeID { get; set; }

    public int? EmployeeIDRequest { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Note { get; set; }
}
