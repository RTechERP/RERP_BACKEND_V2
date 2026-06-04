using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPIEvaluation
{
    public int ID { get; set; }

    public string? EvaluationCode { get; set; }

    public string? Note { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? DepartmentID { get; set; }
}
