using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class WorkPlan
{
    public int ID { get; set; }

    public int? UserID { get; set; }

    public string? WorkContent { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? TotalDay { get; set; }

    public int? STT { get; set; }

    public string? Location { get; set; }

    public int? ProjectID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
