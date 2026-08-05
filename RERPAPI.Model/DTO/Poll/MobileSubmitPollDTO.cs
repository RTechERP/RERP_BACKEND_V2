namespace RERPAPI.Model.DTO.Poll
{
    /// <summary>
    /// DTO for mobile bulk submit poll answers for a single section
    /// </summary>
    public class MobileSubmitPollSectionDTO
    {
        public int? PollResponseID { get; set; }
        public int? SectionID { get; set; }
        public int? EmployeeID { get; set; }
        public List<AnswerItemDTO>? Answers { get; set; }
    }

    /// <summary>
    /// DTO for mobile bulk submit poll answers for multiple sections at once
    /// </summary>
    public class MobileSubmitPollBulkDTO
    {
        public int? PollFormID { get; set; }
        public int? EmployeeID { get; set; }
        public List<MobileSubmitPollBulkSectionDTO>? Sections { get; set; }

        // Flat properties for single section submission from mobile
        public int? PollResponseID { get; set; }
        public int? SectionID { get; set; }
        public List<AnswerItemDTO>? Answers { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("visibleSectionIds")]
        public List<int>? VisibleSectionIds { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("hiddenSectionIds")]
        public List<int>? HiddenSectionIds { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("visibleQuestionIds")]
        public List<int>? VisibleQuestionIds { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("hiddenQuestionIds")]
        public List<int>? HiddenQuestionIds { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("clearSectionIds")]
        public List<int>? ClearSectionIds { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("clearQuestionIds")]
        public List<int>? ClearQuestionIds { get; set; }
    }

    /// <summary>
    /// Each section item inside the bulk submit request
    /// </summary>
    public class MobileSubmitPollBulkSectionDTO
    {
        public int? PollResponseID { get; set; }
        public int? SectionID { get; set; }
        public int? EmployeeID { get; set; }
        public List<AnswerItemDTO>? Answers { get; set; }
    }

    /// <summary>
    /// Result DTO for mobile bulk submit
    /// </summary>
    public class MobileSubmitPollBulkResultDTO
    {
        public int PollResponseID { get; set; }
        public int PollFormID { get; set; }
        public bool IsCompleted { get; set; }
        public int TotalSavedAnswerCount { get; set; }
        public List<MobileSubmitPollBulkSectionResultDTO>? SectionResults { get; set; }
    }

    /// <summary>
    /// Result for each section in the bulk submit
    /// </summary>
    public class MobileSubmitPollBulkSectionResultDTO
    {
        public int SectionID { get; set; }
        public int SavedAnswerCount { get; set; }
    }
}
