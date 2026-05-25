using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

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

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người update
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày update
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Trạng thái xoá
    /// </summary>
    public bool? IsDeleted { get; set; }
}
