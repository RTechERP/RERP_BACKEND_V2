using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TSCalendarPeriodAsset
{
    public int ID { get; set; }

    public string? CodePeriod { get; set; }

    public string? NamePeriod { get; set; }

    public int? DateNumber { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
