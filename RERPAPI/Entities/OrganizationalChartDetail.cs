using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class OrganizationalChartDetail
{
    public int ID { get; set; }

    public int? OrganizationalChartID { get; set; }

    public int? EmployeeID { get; set; }

    public int? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
