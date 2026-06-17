using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ExamResultAnswerDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? ExamResultDetailID { get; set; }

    public int? CourseQuestionID { get; set; }

    public int? CourseAnswerID { get; set; }

    public bool? IsPicked { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
