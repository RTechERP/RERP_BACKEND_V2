namespace RERPAPI.Model.Param
{
    public class ReportImportExportParam
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Find { get; set; }
        public int Group { get; set; }
        public string WareHouseCode { get; set; }
    }
}