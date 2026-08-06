using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ExpectedPayableLog
{
    public int ID { get; set; }

    public int? ExpectedPayableID { get; set; }

    public string? TypeLog { get; set; }

    public string? LogContent { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
