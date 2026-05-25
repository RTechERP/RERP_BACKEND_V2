using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectTaskComplexity
{
    public int ID { get; set; }

    public int? ProjectTaskID { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
