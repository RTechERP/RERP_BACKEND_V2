using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleEmployeeTemplate
{
    public int ID { get; set; }

    public int EmployeeID { get; set; }

    public int TemplateID { get; set; }

    public DateTime AssignedDate { get; set; }

    public string? AssignedBy { get; set; }

    public bool IsActive { get; set; }

    public string? Note { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? PeriodType { get; set; }

    public string? PeriodValue { get; set; }

    public int? PeriodID { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
