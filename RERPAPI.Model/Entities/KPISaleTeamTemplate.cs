using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleTeamTemplate
{
    public int ID { get; set; }

    public int TeamID { get; set; }

    public int TemplateID { get; set; }

    public string? PeriodType { get; set; }

    public string PeriodValue { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? AssignedDate { get; set; }

    public string? AssignedBy { get; set; }

    public bool? IsActive { get; set; }

    public string? Note { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
