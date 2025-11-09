using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ExamQuestion
{
    public int ID { get; set; }

    public int? ExamQuestionTypeID { get; set; }

    public int? STT { get; set; }

    public string? ContentTest { get; set; }

    public string? CorrectAnswer { get; set; }

    public string? Image { get; set; }

    public int? Score { get; set; }
}
