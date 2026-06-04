using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillImportTechDetailSerial
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? BillImportTechDetailID { get; set; }

    public string? SerialNumber { get; set; }

    public int? WarehouseID { get; set; }
}
