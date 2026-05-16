namespace RERPAPI.Model.DTO.Poll
{
    public class PollFormDTO
    {
        public int ID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsDeleted { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class PollFormCreateDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsPublic { get; set; } = true;
    }

    public class PollFormUpdateDTO
    {
        public int ID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsPublic { get; set; }
    }

    public class PollFormDetailDTO : PollFormDTO
    {
        public List<PollQuestionDetailDTO>? Questions { get; set; }
        public List<PollSectionDetailDTO>? Sections { get; set; }
    }
}
