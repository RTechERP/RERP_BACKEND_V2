using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class PercentMainIndexUser
{
    public int ID { get; set; }

    public decimal? PercentIndex { get; set; }

    public int? UserID { get; set; }

    public int? MainIndexID { get; set; }

    public int? Quy { get; set; }

    public int? Year { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
