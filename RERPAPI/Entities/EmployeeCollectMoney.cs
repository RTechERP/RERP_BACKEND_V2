using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class EmployeeCollectMoney
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public bool? IsApproved { get; set; }

    public int? EmployeeID { get; set; }

    public int? PartID { get; set; }

    public decimal? Fund { get; set; }

    public decimal? Error { get; set; }

    public decimal? Party { get; set; }

    public DateTime? CollectDay { get; set; }

    public decimal? TotalMoney { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
