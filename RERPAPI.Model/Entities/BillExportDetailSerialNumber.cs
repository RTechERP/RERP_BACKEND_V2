using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillExportDetailSerialNumber
{
    public int ID { get; set; }

    public int? BillExportDetailID { get; set; }

    public int? STT { get; set; }

    public string? SerialNumber { get; set; }

    public string? SerialNumberRTC { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
