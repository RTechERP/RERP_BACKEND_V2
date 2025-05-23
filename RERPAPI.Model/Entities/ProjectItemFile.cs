using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectItemFile
{
    public int ID { get; set; }

    public int? ProjectItemID { get; set; }

    public string? FileName { get; set; }

    public string? Note { get; set; }

    public string? OriginPath { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? STT { get; set; }
}
