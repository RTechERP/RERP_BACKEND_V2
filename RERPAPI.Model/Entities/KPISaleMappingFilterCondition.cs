using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleMappingFilterCondition
{
    public int ID { get; set; }

    public int FilterGroupID { get; set; }

    public string ColumnName { get; set; } = null!;

    public string Operator { get; set; } = null!;

    public string ValueType { get; set; } = null!;

    public string? Value1 { get; set; }

    public string? Value2 { get; set; }

    public string DataType { get; set; } = null!;

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}
