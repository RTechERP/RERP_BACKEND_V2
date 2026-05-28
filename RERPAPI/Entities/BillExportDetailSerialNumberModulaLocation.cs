using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class BillExportDetailSerialNumberModulaLocation
{
    public int ID { get; set; }

    public int? BillExportDetailSerialNumberID { get; set; }

    public int? ModulaLocationDetailID { get; set; }

    public decimal? Quantity { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? BillExportTechDetailSerialID { get; set; }
}
