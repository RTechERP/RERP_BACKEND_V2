using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectPriority
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public string? ProjectCheckpoint { get; set; }

    public decimal? Rate { get; set; }

    public int? Score { get; set; }

    public decimal? Priority { get; set; }

    public int? ParentID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
