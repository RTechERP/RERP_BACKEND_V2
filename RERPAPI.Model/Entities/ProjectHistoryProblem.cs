using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectHistoryProblem
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? STT { get; set; }

    public string? TypeProblem { get; set; }

    public string? ContentError { get; set; }

    public string? Reason { get; set; }

    public string? Remedies { get; set; }

    public string? TestMethod { get; set; }

    public string? Image { get; set; }

    public DateTime? DateProblem { get; set; }

    public DateTime? DateImplementation { get; set; }

    public string? PIC { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? EmployeeID { get; set; }

    public bool? IsDeleted { get; set; }
}
