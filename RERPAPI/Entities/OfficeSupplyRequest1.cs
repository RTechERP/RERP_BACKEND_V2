using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class OfficeSupplyRequest1
{
    public int ID { get; set; }

    public int? EmployeeIDRequest { get; set; }

    public DateTime? DateRequest { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsApproved { get; set; }

    public int? ApprovedID { get; set; }

    public DateTime? DateApproved { get; set; }

    public int? DepartmentID { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsAdminApproved { get; set; }

    public int? AdminApprovedID { get; set; }

    public DateTime? DateAdminApproved { get; set; }
}
