using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class JobRequirementLog
{
    public int ID { get; set; }

    public int? JobRequirementID { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? DateLog { get; set; }

    public string? LogContent { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
