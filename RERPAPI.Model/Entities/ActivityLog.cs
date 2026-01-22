using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ActivityLog
{
    public int ID { get; set; }

    public DateTime LogTime { get; set; }

    public int? UserID { get; set; }

    public string? Application { get; set; }

    public string? FormName { get; set; }

    public string? Action { get; set; }

    public string? Details { get; set; }

    public int? EmployeeID { get; set; }

    public string? ControlName { get; set; }

    public bool? IsDeleted { get; set; }
}
