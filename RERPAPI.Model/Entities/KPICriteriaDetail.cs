using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPICriteriaDetail
{
    public int ID { get; set; }

    public int? KPICriteriaID { get; set; }

    public int? STT { get; set; }

    public int? Point { get; set; }

    public int? PointPercent { get; set; }

    public string? CriteriaContent { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
