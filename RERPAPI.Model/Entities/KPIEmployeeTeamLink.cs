using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPIEmployeeTeamLink
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public int? KPIEmployeeTeamID { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
