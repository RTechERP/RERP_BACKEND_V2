using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleIndexDataMapping
{
    public int ID { get; set; }

    public int KpiIndexID { get; set; }

    public int DataSourceID { get; set; }

    public string AggregateType { get; set; } = null!;

    public string? ValueColumn { get; set; }

    public string? DistinctColumn { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
