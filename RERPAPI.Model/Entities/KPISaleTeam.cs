using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleTeam
{
    public int ID { get; set; }

    public string TeamCode { get; set; } = null!;

    public string TeamName { get; set; } = null!;

    public string? Description { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsActive { get; set; }

    public int? LeaderEmployeeID { get; set; }
}
