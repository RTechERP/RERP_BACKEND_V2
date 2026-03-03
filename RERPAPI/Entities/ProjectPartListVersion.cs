using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectPartListVersion
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

    public int? ProjectSolutionID { get; set; }

    public int? ProjectTypeID { get; set; }

    public int? StatusVersion { get; set; }

    public bool? IsApproved { get; set; }

    public int? ApprovedID { get; set; }

    public bool? IsDeleted { get; set; }

    public string? ReasonDeleted { get; set; }
}
