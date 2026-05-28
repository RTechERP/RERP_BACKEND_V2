using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ESLConfig
{
    public int ID { get; set; }

    public string ConfigKey { get; set; } = null!;

    public string ConfigValue { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }
}
