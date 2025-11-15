using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HandoverWork
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? HandoverID { get; set; }

    public int? EmployeeID { get; set; }

    public string? ContentWork { get; set; }

    public int? Status { get; set; }

    public string? Frequency { get; set; }

    public string? FileName { get; set; }

    public string? SignedBy { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsSigned { get; set; }
}
