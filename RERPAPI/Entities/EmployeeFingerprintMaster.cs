using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class EmployeeFingerprintMaster
{
    public int ID { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    public bool? IsBrowser { get; set; }

    public string? Note { get; set; }
}
