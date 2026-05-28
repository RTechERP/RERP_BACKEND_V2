using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class CourseExam
{
    public int ID { get; set; }

    public int? CourseId { get; set; }

    public string? NameExam { get; set; }

    public string? CodeExam { get; set; }

    public decimal? Goal { get; set; }

    public int? TestTime { get; set; }

    /// <summary>
    /// 1: trắc nghiệm; 2: Thực hành
    /// </summary>
    public int? ExamType { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? LessonID { get; set; }
}
