using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Course_KPIPositionType
{
    public int ID { get; set; }

    public int? CourseID { get; set; }

    public string? KPIPositionTypeID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
