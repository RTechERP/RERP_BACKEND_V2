using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class TaxEmployeePosition
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? PriorityOrder { get; set; }
}
