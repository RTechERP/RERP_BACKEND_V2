namespace RERPAPI.Model.Param
{
    public class DailyReportHrRequestParam
    {
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public string keyword { get; set; } = "";
        public int departmentID { get; set; } = 6;
        public int userID { get; set; } = 0;
        public int employeeID { get; set; } = 0;
    }
}