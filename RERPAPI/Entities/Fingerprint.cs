using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class Fingerprint
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? UserID { get; set; }

    public string? Organization { get; set; }

    public string? DayOfWeek { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public string? Note { get; set; }

    public string? IDChamCong { get; set; }

    public bool? IsLate { get; set; }

    public bool? IsEarly { get; set; }

    public decimal? SumLate { get; set; }

    public decimal? SumEarly { get; set; }
}
