using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CourseAnswer
{
    public int ID { get; set; }

    public string? AnswerText { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? CourseQuestionId { get; set; }

    public int? AnswerNumber { get; set; }
}
