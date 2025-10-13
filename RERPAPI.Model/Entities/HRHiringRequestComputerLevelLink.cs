using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HRHiringRequestComputerLevelLink
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int HRHiringRequestID { get; set; }

    public int ComputerType { get; set; }

    public string? ComputerName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }
}
