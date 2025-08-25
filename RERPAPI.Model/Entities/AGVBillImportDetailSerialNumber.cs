using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AGVBillImportDetailSerialNumber
{
    public int ID { get; set; }

    public int? AGVBillImportDetailID { get; set; }

    public int? STT { get; set; }

    public string? SerialNumber { get; set; }

    public string? SerialNumberAGV { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
