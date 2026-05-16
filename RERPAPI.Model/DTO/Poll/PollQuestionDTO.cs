namespace RERPAPI.Model.DTO.Poll
{
    public class PollQuestionDTO
    {
        public int ID { get; set; }
        public int? PollFormID { get; set; }
        public int? SectionID { get; set; }
        public string? QuestionText { get; set; }
        public string? FieldKey { get; set; }
        public string? QuestionType { get; set; } // Text, SingleChoice, MultipleChoice, Rating, Date, etc.
        public bool IsRequired { get; set; }
        public int? SortOrder { get; set; }
        public string? ConfigJson { get; set; }
        public string? DataSourceType { get; set; }
        public string? DataSourceField { get; set; }
    }

    public class PollQuestionCreateDTO
    {
        public int? PollFormID { get; set; }
        public int? SectionID { get; set; }
        public string? QuestionText { get; set; }
        public string? FieldKey { get; set; }
        public string? QuestionType { get; set; }
        public bool IsRequired { get; set; } = true;
        public int? SortOrder { get; set; }
        public string? ConfigJson { get; set; }
        public string? DataSourceType { get; set; }
        public string? DataSourceField { get; set; }
    }

    public class PollQuestionUpdateDTO
    {
        public int ID { get; set; }
        public int? SectionID { get; set; }
        public string? QuestionText { get; set; }
        public string? FieldKey { get; set; }
        public string? QuestionType { get; set; }
        public bool? IsRequired { get; set; }
        public int? SortOrder { get; set; }
        public string? ConfigJson { get; set; }
        public string? DataSourceType { get; set; }
        public string? DataSourceField { get; set; }
    }

    public class PollQuestionDetailDTO : PollQuestionDTO
    {
        public string? DataSourceLabel { get; set; }
        public string? DataSourceValue { get; set; }
        public bool IsAutoFilled { get; set; }
        public List<PollQuestionOptionDTO>? Options { get; set; }
    }
}
