using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ExamTestResult
{
    public int ID { get; set; }

    public int? ExamCategoryID { get; set; }

    public int? ExamListTestID { get; set; }

    public int? ExamQuestionBankID { get; set; }

    public int? EmployeeID { get; set; }

    public string? CandidateName { get; set; }

    public string? ResultChose { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }
}
