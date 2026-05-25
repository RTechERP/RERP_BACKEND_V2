namespace RERPAPI.Model.DTO.Poll
{
    public class PollQuestionOptionDTO
    {
        public int ID { get; set; }
        public int? PollQuestionID { get; set; }
        public string? OptionText { get; set; }
        public string? OptionValue { get; set; }
        public int? SortOrder { get; set; }
    }

    public class PollQuestionOptionCreateDTO
    {
        public int? PollQuestionID { get; set; }
        public string? OptionText { get; set; }
        public string? OptionValue { get; set; }
        public int? SortOrder { get; set; }
    }

    public class PollQuestionOptionUpdateDTO
    {
        public int ID { get; set; }
        public string? OptionText { get; set; }
        public string? OptionValue { get; set; }
        public int? SortOrder { get; set; }
    }
}
