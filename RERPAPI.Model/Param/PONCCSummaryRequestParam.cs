namespace RERPAPI.Model.Param
{
    public class PONCCSummaryRequestParam
    {
        public DateTime? @DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string FilterText { get; set; } = "";

        public int SupplierID { get; set; }
        public int Status { get; set; }
        public int EmployeeID { get; set; }
    }
}