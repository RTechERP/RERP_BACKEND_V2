using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HRHiringRequestHealthLink
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int HRHiringRequestID { get; set; }

    public string? HealthDecription { get; set; }

    public int HealthType { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }
}
