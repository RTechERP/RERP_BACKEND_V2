using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ChangeLogStore
{
    public int LogId { get; set; }

    public string DatabaseName { get; set; } = null!;

    public string EventType { get; set; } = null!;

    public string ObjectName { get; set; } = null!;

    public string ObjectType { get; set; } = null!;

    public string SqlCommand { get; set; } = null!;

    public DateTime EventDate { get; set; }

    public string LoginName { get; set; } = null!;
}
