using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities.RTCCourse;

public partial class CourseRightAnswer
{
    public int ID { get; set; }

    public int? CourseQuestionID { get; set; }

    public int? CourseAnswerID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
