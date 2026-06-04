using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ExamQuestionGroup
{
    public int ID { get; set; }

    public string? GroupCode { get; set; }

    public string? GroupName { get; set; }

    public int? DepartmentID { get; set; }
}
