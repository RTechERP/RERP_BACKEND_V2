using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectTypeAssign
{
    public int ID { get; set; }

    public int? ProjectTypeID { get; set; }

    public int? EmployeeID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
