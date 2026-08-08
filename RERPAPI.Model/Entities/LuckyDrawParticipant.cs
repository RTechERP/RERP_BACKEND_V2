using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class LuckyDrawParticipant
{
    public int ID { get; set; }

    public int SessionID { get; set; }

    public int EmployeeID { get; set; }

    public DateTime JoinedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
