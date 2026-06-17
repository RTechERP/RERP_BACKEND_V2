using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectItemDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? ProjectItemID { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? Type { get; set; }

    public int? ParentID { get; set; }

    public bool? HasChild { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
