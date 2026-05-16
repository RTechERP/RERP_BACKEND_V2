namespace RERPAPI.Model.DTO.Poll
{
    public class PollResponseDTO
    {
        public int ID { get; set; }
        public int? PollFormID { get; set; }
        public int? EmployeeID { get; set; }
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
}
