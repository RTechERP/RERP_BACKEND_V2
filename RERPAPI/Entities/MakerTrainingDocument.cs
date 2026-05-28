using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class MakerTrainingDocument
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? MakerTrainingID { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
