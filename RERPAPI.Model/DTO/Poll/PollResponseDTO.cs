namespace RERPAPI.Model.DTO.Poll
{
    public class PollResponseDTO
    {
        public int ID { get; set; }
        public int? PollFormID { get; set; }
        public int? EmployeeID { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class PollResponseCreateDTO
    {
        public int? PollFormID { get; set; }
        public int? EmployeeID { get; set; }
    }

    public class PollResponseDetailDTO : PollResponseDTO
    {
        public List<PollResponseAnswerDTO>? Answers { get; set; }
    }

    public class PollEmployeeResponseStatusDTO
    {
        public int PollFormID { get; set; }
        public int EmployeeID { get; set; }
        public bool HasResponse { get; set; }
        public bool IsCompleted { get; set; }
        public bool CanEdit { get; set; }
        public bool IsClosed { get; set; }
        public string? ClosedReason { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PollResponseDetailDTO? Response { get; set; }
    }
}
