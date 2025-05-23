using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ExamTestResultMaster
{
    public int ID { get; set; }

    public int? ExamCategoryID { get; set; }

    public int? ExamListTestID { get; set; }

    public int? EmployeeID { get; set; }

    public int? TotalQuestion { get; set; }

    public int? TotalChose { get; set; }

    public int? TotalCorrect { get; set; }

    public int? TotalIncorrect { get; set; }

    public decimal? TotalMarks { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }
}
