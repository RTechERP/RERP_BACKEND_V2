using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CourseLessonLog
{
    public int ID { get; set; }

    public int? CourseLessonID { get; set; }

    public DateTime? DateLog { get; set; }

    public string? ContentLog { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
