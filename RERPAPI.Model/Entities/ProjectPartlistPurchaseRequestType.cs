using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectPartlistPurchaseRequestType
{
    public int ID { get; set; }

    public string? RequestTypeName { get; set; }

    public bool? IsIgnoreBGD { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? RequestTypeCode { get; set; }
}
