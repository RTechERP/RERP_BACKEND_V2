using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectStatusLog
{
    public int ID { get; set; }

    public int ProjectID { get; set; }

    public int ProjectStatusID { get; set; }

    public int EmployeeID { get; set; }

    public DateTime DateLog { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;
}
