using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CurrencyConfig
{
    public string Id { get; set; } = null!;

    public string Text { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public string? SubUnit { get; set; }

    public int SortOrder { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
