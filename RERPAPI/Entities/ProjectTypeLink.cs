using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectTypeLink
{
    public int ID { get; set; }

    public int? ProjectTypeID { get; set; }

    public int? ProjectID { get; set; }

    public int? LeaderID { get; set; }

    public bool? Selected { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
