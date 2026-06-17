using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class VisitFactory
{
    public int ID { get; set; }

    public DateTime DateVisit { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public string Purpose { get; set; } = null!;

    public string? Note { get; set; }

    public int EmployeeID { get; set; }

    public string Company { get; set; } = null!;

    public string GuestTypeID { get; set; } = null!;

    public int TotalPeople { get; set; }

    public bool IsReceive { get; set; }

    public int? EmployeeReceive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<VisitFactoryDetail> VisitFactoryDetails { get; set; } = new List<VisitFactoryDetail>();
}
