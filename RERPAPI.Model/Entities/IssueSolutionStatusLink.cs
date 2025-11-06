using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class IssueSolutionStatusLink
{
    public int ID { get; set; }

    public int? StatusID { get; set; }

    public int? IssueSolutionID { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
