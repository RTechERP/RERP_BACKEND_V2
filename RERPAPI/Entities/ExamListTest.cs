using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ExamListTest
{
    public int ID { get; set; }

    public int? ExamTypeTestID { get; set; }

    public int? ExamCategoryID { get; set; }

    public string? CodeTest { get; set; }

    public string? NameTest { get; set; }

    public string? Note { get; set; }

    public int? TestTime { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
