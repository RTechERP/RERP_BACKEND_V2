using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleApprovalLog
{
    public int ID { get; set; }

    public int EmployeeID { get; set; }

    public int PeriodID { get; set; }

    public string ActionType { get; set; } = null!;

    public string StepCode { get; set; } = null!;

    public string PerformedBy { get; set; } = null!;

    public DateTime PerformedDate { get; set; }

    public string? Note { get; set; }

    public string? StatusBefore { get; set; }

    public string? StatusAfter { get; set; }
}
