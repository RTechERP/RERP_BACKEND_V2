using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities.RTCCourse;

public partial class CourseRating
{
    public int ID { get; set; }

    public int CourseID { get; set; }

    public int EmployeeID { get; set; }

    public byte Stars { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
