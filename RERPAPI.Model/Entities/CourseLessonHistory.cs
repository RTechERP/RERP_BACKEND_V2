using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CourseLessonHistory
{
    public int ID { get; set; }

    public int? EmployeeId { get; set; }

    public int? Status { get; set; }

    public int? LessonId { get; set; }

    public DateTime? ViewDate { get; set; }
}
