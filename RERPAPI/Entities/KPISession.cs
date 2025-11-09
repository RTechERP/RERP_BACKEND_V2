using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPISession
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int? YearEvaluation { get; set; }

    public int? QuarterEvaluation { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? DepartmentID { get; set; }
}
