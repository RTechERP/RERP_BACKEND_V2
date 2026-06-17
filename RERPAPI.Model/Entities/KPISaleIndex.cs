using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleIndex
{
    public int ID { get; set; }

    public int TemplateID { get; set; }

    public int? ParentID { get; set; }

    public string IndexCode { get; set; } = null!;

    public string IndexName { get; set; } = null!;

    public string IndexType { get; set; } = null!;

    public string UnitType { get; set; } = null!;

    public decimal WeightPercent { get; set; }

    public string QuarterGoalCalculateType { get; set; } = null!;

    public string QuarterResultCalculateType { get; set; } = null!;

    public int SortOrder { get; set; }

    public bool IsBold { get; set; }

    public bool IsMainIndex { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
