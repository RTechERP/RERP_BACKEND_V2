using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CourseExamResultDetail
{
    public int ID { get; set; }

    public int? CourseQuestionId { get; set; }

    public int? CourseAnswerId { get; set; }

    public int? CourseExamResultId { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
