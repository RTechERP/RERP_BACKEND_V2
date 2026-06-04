using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ReportPurchase
{
    public int ID { get; set; }

    public int? WorkingDays { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public int? UserID { get; set; }

    public int? NoReport { get; set; }

    public int? Quy { get; set; }
}
