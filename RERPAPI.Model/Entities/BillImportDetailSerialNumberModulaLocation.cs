using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillImportDetailSerialNumberModulaLocation
{
    public int ID { get; set; }

    public int? BillImportDetailSerialNumberID { get; set; }

    public int? ModulaLocationDetailID { get; set; }

    public decimal? Quantity { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? AGVBillImportDetailSerial { get; set; }

    public int? BillImportTechDetailSerialID { get; set; }
}
