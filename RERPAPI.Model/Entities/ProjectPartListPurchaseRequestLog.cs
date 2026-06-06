using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectPartListPurchaseRequestLog
{
    public int ID { get; set; }

    public int? ProjectPartListPurchaseRequestID { get; set; }

    public string? TypeLog { get; set; }

    public string? ContentLog { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
