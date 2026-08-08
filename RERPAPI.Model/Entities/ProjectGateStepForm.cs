using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectGateStepForm
{
    public int ID { get; set; }

    public int ProjectGateStepID { get; set; }

    public int? STT { get; set; }

    public string FormName { get; set; } = null!;

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public long? FileSize { get; set; }

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
