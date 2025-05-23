using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectSurvey
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? ProjectID { get; set; }

    /// <summary>
    /// Nhân viên Sale
    /// </summary>
    public int? EmployeeID { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public string? Address { get; set; }

    public string? PIC { get; set; }

    public string? Description { get; set; }

    public string? Note { get; set; }

    public bool? IsUrgent { get; set; }

    public bool? IsApprovedUrgent { get; set; }

    public int? ApprovedUrgentID { get; set; }

    public string? ReasonUrgent { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? PhoneNumber { get; set; }
}
