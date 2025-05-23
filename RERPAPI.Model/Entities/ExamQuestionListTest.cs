using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ExamQuestionListTest
{
    public int ID { get; set; }

    public int? ExamQuestionID { get; set; }

    public int? ExamListTestID { get; set; }

    public int? STT { get; set; }
}
