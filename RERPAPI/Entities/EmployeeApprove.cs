using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class EmployeeApprove
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public string? Code { get; set; }

    public string? FullName { get; set; }

    /// <summary>
    /// 1: TBP duyệt; 2: leader dự án
    /// </summary>
    public int? Type { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UsersID { get; set; }

    public bool? IsPassed { get; set; }
}
