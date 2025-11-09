using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class InventoryProjectExport
{
    public int ID { get; set; }

    public int? InventoryProjectID { get; set; }

    public int? BillExportDetailID { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public decimal? Quantity { get; set; }
}
