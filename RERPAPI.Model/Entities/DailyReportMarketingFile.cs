using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class DailyReportMarketingFile
{
    public int ID { get; set; }

    public string FileName { get; set; } = null!;

    public int DailyReportID { get; set; }

    public string? PathServer { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }
}
