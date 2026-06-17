using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillExportTechnicalLog
{
    public int ID { get; set; }

    public int? BillExportTechnicalID { get; set; }

    public bool? StatusBill { get; set; }

    public DateTime? DateStatus { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
