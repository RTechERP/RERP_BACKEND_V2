namespace RERPAPI.Model.DTO
{
    public class GradeEssayRequestDTO
    {
        public int ExamResultDetailID { get; set; }
        public decimal Score { get; set; }
    }

    public class FinalizeGradingRequestDTO
    {
        public int ExamResultID { get; set; }
    }
}
