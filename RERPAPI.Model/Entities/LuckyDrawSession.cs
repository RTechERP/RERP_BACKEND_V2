using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class LuckyDrawSession
{
    public int ID { get; set; }

    public string SessionCode { get; set; } = null!;

    public string SessionName { get; set; } = null!;

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? Status { get; set; }

    public int WinnerCount { get; set; }

    public DateTime? ClosedAt { get; set; }

    public DateTime? DrawAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
