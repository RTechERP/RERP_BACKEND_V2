using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CourseExamPractice
{
    public int ID { get; set; }

    public int? CourseID { get; set; }

    public int? EmployeeID { get; set; }

    public decimal? PracticePoints { get; set; }

    /// <summary>
    /// 1; đánh giá Đạt; 0:Không đạt
    /// </summary>
    public bool? Evaluate { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
