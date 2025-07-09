using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeScheduleWork
{
    public int ID { get; set; }

    public DateTime? DateValue { get; set; } 

    public bool? Status { get; set; }

    public int? WorkDay { get; set; }

    public int? WorkMonth { get; set; }

    public int? WorkYear { get; set; }

    public bool IsApproved { get; set; }

    public int? Approver { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
