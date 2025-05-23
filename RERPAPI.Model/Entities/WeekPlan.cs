using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class WeekPlan
{
    public int ID { get; set; }

    public DateTime? DatePlan { get; set; }

    public int? UserID { get; set; }

    public string? ContentPlan { get; set; }

    public string? Result { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
