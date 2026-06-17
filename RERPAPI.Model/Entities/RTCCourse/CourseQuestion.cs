using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities.RTCCourse;

public partial class CourseQuestion
{
    public int ID { get; set; }

    public string? QuestionText { get; set; }

    public int? STT { get; set; }

    public int? CourseExamId { get; set; }

    public int? CheckInput { get; set; }

    public int? Marks { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Image { get; set; }
}
