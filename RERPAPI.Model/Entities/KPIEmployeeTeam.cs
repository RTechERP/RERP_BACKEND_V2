using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPIEmployeeTeam
{
    public int ID { get; set; }

    public string? Name { get; set; }

    public int? DepartmentID { get; set; }

    public int? LeaderID { get; set; }

    public int? ParentID { get; set; }

    public int? YearValue { get; set; }

    public int? QuarterValue { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
