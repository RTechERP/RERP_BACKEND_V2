using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectTask
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? ProjectTaskGroupID { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? EmployeeID { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public int? Priority { get; set; }

    public int? Status { get; set; }

    public int? OrderIndex { get; set; }

    public int? ParentID { get; set; }

    public int? ProgressPercent { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
