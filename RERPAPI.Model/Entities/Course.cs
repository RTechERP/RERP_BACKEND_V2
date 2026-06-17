using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Course
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? Code { get; set; }

    public string? NameCourse { get; set; }

    public string? Instructor { get; set; }

    public int? CourseCatalogID { get; set; }

    public bool? DeleteFlag { get; set; }

    public int? FileCourseID { get; set; }

    public bool? IsPractice { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public decimal? QuestionCount { get; set; }

    public decimal? QuestionDuration { get; set; }

    public decimal? LeadTime { get; set; }

    public int? CourseCopyID { get; set; }

    public int? CourseTypeID { get; set; }

    public int? EmployeeID { get; set; }
}
