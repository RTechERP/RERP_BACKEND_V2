using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Goal
{
    public int ID { get; set; }

    public decimal? Goal0 { get; set; }

    public int? UserID { get; set; }

    public int? MainIndexID { get; set; }

    public int? MonthReport { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public decimal? Goal1 { get; set; }

    public decimal? Goal2 { get; set; }

    public int? Quy { get; set; }
}
