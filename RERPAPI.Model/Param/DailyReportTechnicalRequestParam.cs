namespace RERPAPI.Model.Param
{
    public class DailyReportTechnicalRequestParam
    {
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public string keyword { get; set; } = "";
        public int departmenID { get; set; } = 6;
        public int userID { get; set; } = 0;
    }
}