using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class JobRequirementComment
{
    public int ID { get; set; }

    public int? JobRequirementID { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? DateComment { get; set; }

    public string? CommentContent { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
