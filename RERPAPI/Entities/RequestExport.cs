using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RequestExport
{
    public int ID { get; set; }

    public bool? IsApproved { get; set; }

    public int? SupplierID { get; set; }

    public string? RequestCode { get; set; }

    public string? RequestName { get; set; }

    public string? UserRequest { get; set; }

    public string? UserExport { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ExportDate { get; set; }

    public string? Note { get; set; }
}
