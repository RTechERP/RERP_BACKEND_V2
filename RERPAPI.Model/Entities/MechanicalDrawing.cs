using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class MechanicalDrawing
{
    public int ID { get; set; }

    public string? Name { get; set; }

    public bool? IsDeleted { get; set; }

    public int? ProjectID { get; set; }

    public string? FilePath { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? ThumbnailPath { get; set; }

    public int? ProjectTypeID { get; set; }
}
