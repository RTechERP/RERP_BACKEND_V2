using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeOvertimeProjectItem
{
    public int ID { get; set; }

    public int? EmployeeOvertimeID { get; set; }

    public int? ProjectItemID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
