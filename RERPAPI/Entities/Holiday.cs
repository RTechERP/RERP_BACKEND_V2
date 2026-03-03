using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class Holiday
{
    public int ID { get; set; }

    public DateOnly? HolidayDate { get; set; }

    public int? HolidayYear { get; set; }

    public int? HolidayMonth { get; set; }

    public int? HolidayDay { get; set; }

    public string? DayValue { get; set; }

    public string? HolidayName { get; set; }

    public string? HolidayCode { get; set; }

    public string? Note { get; set; }

    /// <summary>
    /// 1: Nghỉ có hưởng lương, 2: Nghỉ không hưởng lương
    /// </summary>
    public int? TypeHoliday { get; set; }
}
