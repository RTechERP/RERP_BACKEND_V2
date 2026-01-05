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
}
