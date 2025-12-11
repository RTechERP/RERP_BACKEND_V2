using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class WorkPlanDetail
{
    public int ID { get; set; }

    public int? UserID { get; set; }

    public string? WorkContent { get; set; }

    public DateTime? DateDay { get; set; }

    public string? Location { get; set; }

    public int? ProjectID { get; set; }

    public int? STT { get; set; }

    public int? WorkPlanID { get; set; }

    public string? WorkCode { get; set; }

    public int? Status { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
