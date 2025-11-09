using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class TrackingMarksLog
{
    public int ID { get; set; }

    public int? TrackingMarksID { get; set; }

    public DateTime? DateLog { get; set; }

    public int? EmployeeID { get; set; }

    public string? ContentLog { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
