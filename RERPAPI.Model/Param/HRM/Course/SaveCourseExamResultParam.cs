using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class SaveCourseExamResultParam
{
    public int CourseId { get; set; }
    public int ExamType { get; set; }
    public CourseExamResult CourseExamResult { get; set; } = new CourseExamResult();
}
