using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeOnLeaveMaster
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    /// <summary>
    /// Số ngày phép trong năm
    /// </summary>
    public decimal? TotalDayInYear { get; set; }

    /// <summary>
    /// Số ngày đã nghỉ phép 
    /// </summary>
    public decimal? TotalDayOnLeave { get; set; }

    /// <summary>
    /// Số ngày nghỉ không phép
    /// </summary>
    public decimal? TotalDayNoOnLeave { get; set; }

    /// <summary>
    /// Số ngày phép còn lại
    /// </summary>
    public decimal? TotalDayRemain { get; set; }

    public int? YearOnleave { get; set; }

    public bool IsDeleted { get; set; }
}
