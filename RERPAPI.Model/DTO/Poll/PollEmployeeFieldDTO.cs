namespace RERPAPI.Model.DTO.Poll
{
    public class PollEmployeeFieldOptionDTO
    {
        public string FieldKey { get; set; } = "";
        public string Label { get; set; } = "";
        public string DataType { get; set; } = "";
        public string SuggestedQuestionType { get; set; } = "Text";
        public bool IsSensitive { get; set; }
    }
}
