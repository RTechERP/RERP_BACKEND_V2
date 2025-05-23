using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class OrganizationalChart
{
    public int ID { get; set; }

    public int? TaxCompanyID { get; set; }

    public int? DepartmentID { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int? ParentID { get; set; }

    public int? IsDeleted { get; set; }

    /// <summary>
    /// ID leader
    /// </summary>
    public int? EmployeeID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
