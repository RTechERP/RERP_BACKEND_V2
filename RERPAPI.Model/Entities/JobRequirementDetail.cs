using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class JobRequirementDetail
{
    public int ID { get; set; }

    public int? JobRequirementID { get; set; }

    public int? STT { get; set; }

    public string? Category { get; set; }

    public string? Description { get; set; }

    public string? Target { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
