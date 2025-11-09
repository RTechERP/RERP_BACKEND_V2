using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class RegisterIdea
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? DateRegister { get; set; }

    public bool? IsApprovedTBP { get; set; }

    public DateTime? DateApprovedTBP { get; set; }

    public int? ApprovedTBPID { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? DateApproved { get; set; }

    public int? ApprovedID { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? RegisterIdeaTypeID { get; set; }

    public int? DepartmentOrganizationID { get; set; }

    public int? CourseID { get; set; }
}
