using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ReportType
{
    public int ID { get; set; }

    public string ReportTypeName { get; set; } = null!;

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
