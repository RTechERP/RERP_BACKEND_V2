namespace RERPAPI.Model.Param.HRM.JobPerfomanceEvaluation
{
    public class GetContactTransferReviewParam
    {
        public int? EmployeeID { get; set; }

        public int? DepartmentID { get; set; }

        public int? Step { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string? Keyword { get; set; }

        public string? Role { get; set; }
    }
}