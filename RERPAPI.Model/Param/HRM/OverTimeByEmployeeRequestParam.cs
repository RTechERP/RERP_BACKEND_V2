namespace RERPAPI.Model.Param.HRM
{
    public class OverTimeByEmployeeRequestParam
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? EmployeeID { get; set; }
        public int? IsApprove { get; set; }
        public string? KeyWord { get; set; }

        public int? Type { get; set; }
    }
}