using System.Text.Json.Serialization;

namespace RERPAPI.Model.DTO.Poll
{
    public class PollSectionDTO
    {
        public int ID { get; set; }
        public int? PollFormID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public string? ShowIfJson { get; set; }
        public string? BranchingRulesJson { get; set; }
        public bool? IsDeleted { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class PollSectionCreateDTO
    {
        public int? PollFormID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public string? ShowIfJson { get; set; }
        public string? BranchingRulesJson { get; set; }
    }

    public class PollSectionUpdateDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public string? ShowIfJson { get; set; }
        public string? BranchingRulesJson { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class PollSectionDetailDTO : PollSectionDTO
    {
        public List<PollQuestionDetailDTO>? Questions { get; set; }
    }

    public class SubmitPollSectionDTO
    {
        public int? PollResponseID { get; set; }
        public int? SectionID { get; set; }
        public int? EmployeeID { get; set; }
        public List<AnswerItemDTO>? Answers { get; set; }
        [JsonPropertyName("hiddenSectionIds")]
        public List<int>? HiddenSectionIds { get; set; }
        [JsonPropertyName("hiddenQuestionIds")]
        public List<int>? HiddenQuestionIds { get; set; }
        [JsonPropertyName("clearSectionIds")]
        public List<int>? ClearSectionIds { get; set; }
        [JsonPropertyName("clearQuestionIds")]
        public List<int>? ClearQuestionIds { get; set; }
    }

    public class SubmitPollSectionResultDTO
    {
        public int PollResponseID { get; set; }
        public int PollFormID { get; set; }
        public int SectionID { get; set; }
        public int? NextSectionID { get; set; }
        public bool IsCompleted { get; set; }
        public int SavedAnswerCount { get; set; }
    }
}
