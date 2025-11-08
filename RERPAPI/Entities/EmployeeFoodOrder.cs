using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class EmployeeFoodOrder
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public int Quantity { get; set; }

    public DateTime? DateOrder { get; set; }

    public string? Note { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public int? DecilineApprove { get; set; }

    public string? ReasonDeciline { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    /// 1: VP Hà nội, 2: Đan phượng
    /// </summary>
    public int? Location { get; set; }
}
