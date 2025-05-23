using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class FollowProjectBaseDetail
{
    public int ID { get; set; }

    public int? FollowProjectBaseID { get; set; }

    public int? ProjectID { get; set; }

    public int? UserID { get; set; }

    public DateTime? ImplementationDate { get; set; }

    public DateTime? ExpectedDate { get; set; }

    public string? WorkDone { get; set; }

    public string? WorkWillDo { get; set; }

    public string? Results { get; set; }

    public string? ProblemBacklog { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
