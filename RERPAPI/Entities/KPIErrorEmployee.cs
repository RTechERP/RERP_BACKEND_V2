using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class KPIErrorEmployee
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? KPIErrorID { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? ErrorDate { get; set; }

    public int? ErrorNumber { get; set; }

    public string? Note { get; set; }

    public bool? IsDelete { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public decimal? TotalMoney { get; set; }
}
