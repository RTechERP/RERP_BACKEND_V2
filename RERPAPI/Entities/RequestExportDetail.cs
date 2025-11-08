using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RequestExportDetail
{
    public int ID { get; set; }

    public int? RequestID { get; set; }

    public int? ProductID { get; set; }

    public int? POKHID { get; set; }

    public int? Qty { get; set; }

    /// <summary>
    /// kho
    /// </summary>
    public string? Warehouse { get; set; }

    public string? Project { get; set; }

    public string? Note { get; set; }
}
