using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class CourseKPIEmployeeTeamMap
{
    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID khoá học
    /// </summary>
    public int CourseID { get; set; }

    /// <summary>
    /// ID Course KPI Employee Team
    /// </summary>
    public int KPIEmployeeTeamID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
