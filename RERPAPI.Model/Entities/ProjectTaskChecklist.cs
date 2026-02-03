using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectTaskChecklist
{
    public int ID { get; set; }

    public int? ProjectTaskID { get; set; }

    public string? ChecklistTitle { get; set; }

    public int? OrderIndex { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsDone { get; set; }
}
