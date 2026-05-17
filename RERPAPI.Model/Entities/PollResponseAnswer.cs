using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PollResponseAnswer
{
    public int ID { get; set; }

    public int? PollResponseID { get; set; }

    public int? PollQuestionID { get; set; }

    public string? AnswerText { get; set; }

    public string? AnswerJson { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? DisplayText { get; set; }
}
