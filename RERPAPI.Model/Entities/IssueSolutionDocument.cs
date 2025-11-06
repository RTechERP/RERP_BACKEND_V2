using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class IssueSolutionDocument
{
    public int ID { get; set; }

    public string? DocumentType { get; set; }

    public string? DocumentNumber { get; set; }

    public int? IssueSolutionID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
