namespace RERPAPI.Model.DTO.Poll
{
    public class PollResponseAnswerDTO
    {
        public int ID { get; set; }
        public int? PollResponseID { get; set; }
        public int? PollQuestionID { get; set; }
        public string? AnswerText { get; set; }
        public string? AnswerJson { get; set; }
    }

    public class PollResponseAnswerCreateDTO
    {
        public int? PollResponseID { get; set; }
        public int? PollQuestionID { get; set; }
        public string? AnswerText { get; set; }
        public string? AnswerJson { get; set; }
    }

    public class SubmitPollResponseDTO
    {
        public int? PollFormID { get; set; }
        public int? EmployeeID { get; set; }
        public List<AnswerItemDTO>? Answers { get; set; }
    }

    public class AnswerItemDTO
    {
        public int? QuestionID { get; set; }
        public string? AnswerText { get; set; }
        public string? AnswerJson { get; set; }
    }
}
