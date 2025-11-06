using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeChucVu
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? PriorityOrder { get; set; }

    public bool? IsBusinessCost { get; set; }

    public bool IsDeleted { get; set; }
}
