using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillDocumentImportTechnicalLog
{
    public int ID { get; set; }

    public int? BillDocumentImportTechnicalID { get; set; }

    public int? Status { get; set; }

    public DateTime? LogDate { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
