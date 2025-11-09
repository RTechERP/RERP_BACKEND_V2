using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class Inventory
{
    public int ID { get; set; }

    public int? ProductSaleID { get; set; }

    public int? WarehouseID { get; set; }

    public decimal? TotalQuantityFirst { get; set; }

    public decimal? Import { get; set; }

    public decimal? Export { get; set; }

    public decimal? TotalQuantityLast { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsStock { get; set; }

    public decimal? MinQuantity { get; set; }

    public int? EmployeeStock { get; set; }
}
