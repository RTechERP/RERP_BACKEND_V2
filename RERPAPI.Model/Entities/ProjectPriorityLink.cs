using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectPriorityLink
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? ProjectPriorityID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
