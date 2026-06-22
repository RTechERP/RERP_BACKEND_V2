using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities.RTCCourse;

public partial class CourseLessonLike
{
    public int ID { get; set; }

    public int LessonID { get; set; }

    public int EmployeeID { get; set; }

    public DateTime? CreatedDate { get; set; }
}
