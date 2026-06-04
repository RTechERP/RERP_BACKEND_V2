using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AGVBillDocumentImport
{
    public int ID { get; set; }

    public int? AGVBillImportID { get; set; }

    public int? DocumentImportID { get; set; }

    public int? Status { get; set; }

    public DateTime? LogDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Note { get; set; }

    public int? StatusPurchase { get; set; }

    public string? ReasonCancel { get; set; }

    public int? EmployeeReceiveID { get; set; }

    public DateTime? DateReceive { get; set; }

    public bool? IsDeleted { get; set; }
}
