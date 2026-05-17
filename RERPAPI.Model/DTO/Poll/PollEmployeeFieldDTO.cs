namespace RERPAPI.Model.DTO.Poll
{
    public class PollEmployeeFieldOptionDTO
    {
        public string FieldKey { get; set; } = "";
        public string Label { get; set; } = "";
        public string DataType { get; set; } = "";
        public string SuggestedQuestionType { get; set; } = "Text";
        public string DisplayType { get; set; } = "raw"; // raw, lookup, enum
        public string? LookupSource { get; set; }
        public bool IsSensitive { get; set; }
    }
}
