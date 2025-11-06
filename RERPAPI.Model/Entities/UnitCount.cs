using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class UnitCount
{
    public int ID { get; set; }

    public string? UnitCode { get; set; }

    public string? UnitName { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
