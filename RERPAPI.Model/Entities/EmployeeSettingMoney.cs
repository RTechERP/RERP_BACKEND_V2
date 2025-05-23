using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeSettingMoney
{
    public int ID { get; set; }

    public decimal? MoneyOT { get; set; }

    public decimal? MoneyQuaDem { get; set; }

    public decimal? MoneyCTX { get; set; }

    public decimal? MoneyCTG { get; set; }
}
