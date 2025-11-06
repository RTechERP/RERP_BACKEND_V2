using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class InvoiceLink
{
    public int ID { get; set; }

    public int? InvoiceID { get; set; }

    public int? BillImportDetailID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? AccountingBillID { get; set; }

    public bool? IsDeleted { get; set; }
}
