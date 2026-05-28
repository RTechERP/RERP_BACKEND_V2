using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPISpecializationType
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? DepartmentID { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
