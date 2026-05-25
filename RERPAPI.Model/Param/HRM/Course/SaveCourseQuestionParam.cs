using RERPAPI.Model.DTO;
using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class SaveCourseQuestionParam
{
    public int ExamType { get; set; }
    public CourseQuestion Question { get; set; } = new CourseQuestion();
    public List<CourseAnswerDTO> Answers { get; set; } = new List<CourseAnswerDTO>();
    public List<int> DeleteAnswerIds { get; set; } = new List<int>();
}
