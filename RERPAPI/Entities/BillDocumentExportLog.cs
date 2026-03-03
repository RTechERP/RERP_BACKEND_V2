using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class BillDocumentExportLog
{
    public int ID { get; set; }

    public int? BillDocumentExportID { get; set; }

    public int? Status { get; set; }

    public DateTime? LogDate { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
