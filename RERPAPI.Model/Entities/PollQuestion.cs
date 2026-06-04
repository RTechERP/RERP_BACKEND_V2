using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PollQuestion
{
    public int ID { get; set; }

    public int? PollFormID { get; set; }

    public string? QuestionText { get; set; }

    public string? FieldKey { get; set; }

    public string? QuestionType { get; set; }

    public bool IsRequired { get; set; }

    public int? SortOrder { get; set; }

    public string? ConfigJson { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? PollSectionID { get; set; }

    public string? DataSourceType { get; set; }

    public string? DataSourceField { get; set; }
}
