using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ViewDetailKPIPurchase
{
    public int ID { get; set; }

    public string? WeekText { get; set; }

    public int? Total { get; set; }

    public int? NumberDebt { get; set; }

    public int? OnScheduleDelivery { get; set; }

    public int? OnScheduleCheck { get; set; }

    public int? SlowProgressDelivery { get; set; }

    public int? SlowProgressCheck { get; set; }

    public int? SlowProgressAcceptedDelivery { get; set; }

    public int? SlowProgressAcceptedCheck { get; set; }

    public int? CantCheck { get; set; }

    public int? TotalCheck { get; set; }

    public int? GroupKPI { get; set; }

    public int? Discount { get; set; }

    public int? Month { get; set; }

    public int? WeekID { get; set; }

    public int? HistoryMoney { get; set; }
}
