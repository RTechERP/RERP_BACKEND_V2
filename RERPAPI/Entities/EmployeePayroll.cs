using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class EmployeePayroll
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public int? _Month { get; set; }

    public int? _Year { get; set; }

    public string? Note { get; set; }

    public bool? isApproved { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
