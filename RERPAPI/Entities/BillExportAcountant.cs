using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class BillExportAcountant
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public int? UserID { get; set; }

    public int? CustomerID { get; set; }

    public string? Address { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
