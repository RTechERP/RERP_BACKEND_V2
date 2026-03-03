using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPIError
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Content { get; set; }

    public string? Note { get; set; }

    public int? Quantity { get; set; }

    /// <summary>
    /// 1: Lần
    /// </summary>
    public int? Unit { get; set; }

    public decimal? Monney { get; set; }

    public bool? IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? KPIErrorTypeID { get; set; }

    public int? DepartmentID { get; set; }
}
