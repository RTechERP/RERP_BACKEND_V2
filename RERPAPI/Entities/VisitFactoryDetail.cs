using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class VisitFactoryDetail
{
    public int ID { get; set; }

    public int VisitFactoryID { get; set; }

    public int? EmployeeID { get; set; }

    public string FullName { get; set; } = null!;

    public string? Company { get; set; }

    public string? Position { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual VisitFactory VisitFactory { get; set; } = null!;
}
