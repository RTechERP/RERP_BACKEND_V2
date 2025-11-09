using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectWorker
{
    public int ID { get; set; }

    public string? TT { get; set; }

    public string? WorkContent { get; set; }

    public int? AmountPeople { get; set; }

    public decimal? NumberOfDay { get; set; }

    public decimal? TotalWorkforce { get; set; }

    public decimal? Price { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? ParentID { get; set; }

    public bool? IsApprovedTBP { get; set; }

    public int? ProjectWorkerTypeID { get; set; }

    public int? ProjectID { get; set; }

    public bool? IsDeleted { get; set; }

    public int? ProjectWorkerVersionID { get; set; }

    public int? StatusVersion { get; set; }

    public int? ProjectTypeID { get; set; }

    public int? ProjectSolutionID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
