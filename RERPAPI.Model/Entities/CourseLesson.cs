using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CourseLesson
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public string? LessonTitle { get; set; }

    public string? LessonContent { get; set; }

    public int? Duration { get; set; }

    public string? VideoURL { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? STT { get; set; }

    public int? CourseID { get; set; }

    public int? FileCourseID { get; set; }

    public string? UrlPDF { get; set; }

    public int? LessonCopyID { get; set; }

    public bool? IsDeleted { get; set; }
}
