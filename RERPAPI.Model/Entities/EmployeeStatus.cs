using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeStatus
{
    public int ID { get; set; }

    public string? StatusCode { get; set; }

    public string? StatusName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
