using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPISaleAllowedColumn
{
    public int ID { get; set; }

    public int TableID { get; set; }

    public string ColumnName { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string DataType { get; set; } = null!;

    public bool CanFilter { get; set; }

    public bool CanAggregate { get; set; }

    public bool CanDistinct { get; set; }

    public bool IsEmployeeColumn { get; set; }

    public bool IsDateColumn { get; set; }

    public bool IsValueColumn { get; set; }

    public bool IsActive { get; set; }

    public string? ManualValueMapJson { get; set; }

    public string? LookupTable { get; set; }

    public string? LookupValueColumn { get; set; }

    public string? LookupDisplayColumn { get; set; }
}
