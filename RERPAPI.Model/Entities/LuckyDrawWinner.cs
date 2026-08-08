using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class LuckyDrawWinner
{
    public int ID { get; set; }

    public int SessionID { get; set; }

    public int ParticipantID { get; set; }

    public int EmployeeID { get; set; }

    public int WinnerOrder { get; set; }

    public DateTime DrawnAt { get; set; }

    public int Status { get; set; }

    public DateTime? VoidedAt { get; set; }

    public string? VoidedBy { get; set; }

    public string? Note { get; set; }
}
