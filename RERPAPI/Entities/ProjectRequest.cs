using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProjectRequest
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? STT { get; set; }

    public DateTime? DateRequest { get; set; }

    public string? CodeRequest { get; set; }

    public string? ContentRequest { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
