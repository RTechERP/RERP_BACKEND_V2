using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectWorkerVersion
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? STT { get; set; }

    public string? Code { get; set; }

    public string? DescriptionVersion { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ProjectTypeID { get; set; }

    public int? ProjectSolutionID { get; set; }

    public int? StatusVersion { get; set; }

    public bool? IsDeleted { get; set; }

    public int? ProjectHistoryProblemID { get; set; }

    public bool? IsProblem { get; set; }

    public bool? IsApprovedTBP { get; set; }

    public DateTime? ApprovedTBPDate { get; set; }

    public int? ApprovedTBPID { get; set; }
}
