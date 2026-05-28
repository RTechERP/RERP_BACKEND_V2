using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ESLTestTableRegistrationLog
{
    public int ID { get; set; }

    public int RegistrationID { get; set; }

    public string Action { get; set; } = null!;

    public int ActionBy { get; set; }

    public DateTime? ActionDate { get; set; }

    public string? Note { get; set; }

    public int? OldStatus { get; set; }

    public int? NewStatus { get; set; }

    public string? APIResponse { get; set; }

    public virtual ESLTestTableRegistration Registration { get; set; } = null!;
}
