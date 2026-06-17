using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectStatusDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? ProjectID { get; set; }

    public int? ProjectStatusID { get; set; }

    /// <summary>
    /// Ngày bắt đầu dự kiến
    /// </summary>
    public DateTime? EstimatedStartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc dự kiến
    /// </summary>
    public DateTime? EstimatedEndDate { get; set; }

    public DateTime? ActualStartDate { get; set; }

    public DateTime? ActualEndDate { get; set; }

    public bool? Selected { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
