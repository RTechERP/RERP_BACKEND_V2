using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleTarget
{
    public int ID { get; set; }

    public int EmployeeID { get; set; }

    public int PeriodID { get; set; }

    public int KpiIndexID { get; set; }

    public decimal GoalValue { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
