using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AGVBillImportDetailSerial
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? AGVBillImportDetailID { get; set; }

    public string? SerialNumber { get; set; }

    public int? WarehouseID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
