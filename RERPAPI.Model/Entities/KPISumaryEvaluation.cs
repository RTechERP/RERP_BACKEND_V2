using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISumaryEvaluation
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public int? KPIExamID { get; set; }

    public int? SpecializationType { get; set; }

    public decimal? EmployeePoint { get; set; }

    public decimal? TBPPoint { get; set; }

    public decimal? BGDPoint { get; set; }
}
