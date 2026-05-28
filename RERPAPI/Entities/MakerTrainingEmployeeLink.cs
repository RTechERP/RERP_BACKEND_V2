using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class MakerTrainingEmployeeLink
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? MakerTrainingID { get; set; }

    public int? EmployeeID { get; set; }

    public bool? IsPass { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
