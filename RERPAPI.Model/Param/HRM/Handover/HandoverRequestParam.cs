namespace RERPAPI.Model.Param.Handover
{
    public class HandoverRequestParam
    {
        public string Keyword { get; set; } = "";
        public int DepartmentID { get; set; }
        public int EmployeeID { get; set; }
        public int LeaderID { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int ApproverID { get; set; }
    }
}