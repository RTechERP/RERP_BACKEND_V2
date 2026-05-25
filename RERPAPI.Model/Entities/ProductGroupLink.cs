using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProductGroupLink
{
    public int ID { get; set; }

    public int? WarehouseID { get; set; }

    public int? ProductGroupID { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Createdby { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
