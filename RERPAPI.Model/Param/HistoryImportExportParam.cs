namespace RERPAPI.Model.Param
{
    public class HistoryImportExportParam
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string FilterText { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Status { get; set; }
        public string WareHouseCode { get; set; }

        public bool checkedAll { get; set; }
    }
}