using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class DepartmentRequired
{
    public int ID { get; set; }

    public int? JobRequirementID { get; set; }

    public int? RequesterID { get; set; }

    public int? PositionID { get; set; }

    public int? DepartmentID { get; set; }

    public DateTime? RequestDate { get; set; }

    public DateTime? CompletionDate { get; set; }

    public string? RequestContent { get; set; }

    public string? Reason { get; set; }

    public string? Unit { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? STT { get; set; }
}
