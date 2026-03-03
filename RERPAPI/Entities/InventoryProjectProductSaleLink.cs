using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class InventoryProjectProductSaleLink
{
    public int ID { get; set; }

    public int? ProductSaleID { get; set; }

    public int? EmployeeID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
