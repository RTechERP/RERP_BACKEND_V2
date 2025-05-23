using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CourseExamEvaluate
{
    public int ID { get; set; }

    public int? CourseExamResultID { get; set; }

    public int? CourseQuestionID { get; set; }

    public decimal? Point { get; set; }

    public bool Evaluate { get; set; }

    public string? Note { get; set; }

    public DateTime? DateCompleted { get; set; }

    public DateTime? DateEvaluate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
