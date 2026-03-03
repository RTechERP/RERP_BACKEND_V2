using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class HRHiringRequestGenderLink
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? HRHiringRequestID { get; set; }

    public int? Gender { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }
}
