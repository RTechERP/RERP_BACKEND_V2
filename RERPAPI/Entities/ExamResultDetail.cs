using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ExamResultDetail
{
    public int ID { get; set; }

    public int? ExamResultID { get; set; }

    public int? CourseQuestionID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
