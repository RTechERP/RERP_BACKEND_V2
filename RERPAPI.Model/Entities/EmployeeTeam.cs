using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeTeam
{
    public int ID { get; set; }

    public string? Name { get; set; }

    public int? DepartmentID { get; set; }

    public string? Code { get; set; }

    public int? STT { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
