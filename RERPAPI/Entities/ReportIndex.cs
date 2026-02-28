using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ReportIndex
{
    public int ID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UserID { get; set; }

    public int? Quy { get; set; }

    public int? Year { get; set; }

    public int? Month0 { get; set; }

    public int? Month1 { get; set; }

    public int? Month2 { get; set; }

    public int? Month { get; set; }

    public int? KPIID { get; set; }

    public int? WarehouseID { get; set; }
}
