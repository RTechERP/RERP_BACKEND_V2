using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectTypeDepartment
{
    public int ID { get; set; }

    public int? DepartmentID { get; set; }

    public int? ProjectTypeID { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
