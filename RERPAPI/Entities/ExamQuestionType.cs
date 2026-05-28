using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ExamQuestionType
{
    public int ID { get; set; }

    public string? TypeCode { get; set; }

    public string? TypeName { get; set; }

    public int? ExamQuestionGroupID { get; set; }

    public decimal? ScoreRating { get; set; }
}
