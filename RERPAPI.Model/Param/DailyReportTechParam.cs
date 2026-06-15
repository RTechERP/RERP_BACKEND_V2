namespace RERPAPI.Model.Param
{
    public class DailyReportTechParam
    {
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public string keyword { get; set; } = "";
        public int departmentID { get; set; } = 6;
        public int teamID { get; set; } = 0;
        public int userID { get; set; } = 0;
        public int employeeID { get; set; } = 0;
    }
}