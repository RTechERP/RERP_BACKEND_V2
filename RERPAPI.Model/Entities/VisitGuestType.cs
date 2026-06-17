using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class VisitGuestType
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public string? Note { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }
}
