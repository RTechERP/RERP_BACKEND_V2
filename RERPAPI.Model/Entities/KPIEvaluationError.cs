using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPIEvaluationError
{
    public int ID { get; set; }

    public int? KPIEvaluationID { get; set; }

    public int? KPIErrorID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
