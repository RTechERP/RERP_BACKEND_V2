using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ExamResult
{
    public int ID { get; set; }

    public int? YearValue { get; set; }

    /// <summary>
    /// Quý
    /// </summary>
    public int? Season { get; set; }

    /// <summary>
    /// 1=Vision, 2=Điện, 3=PM, 4=Nội Quy
    /// </summary>
    public int? TestType { get; set; }

    public int? EmployeeID { get; set; }

    public int? TotalQuestion { get; set; }

    public int? TotalChoosen { get; set; }

    public int? TotalCorrect { get; set; }

    public int? TotalInCorrect { get; set; }

    public decimal? TotalMarks { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
