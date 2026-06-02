namespace RERPAPI.Model.Param
{
    public class OfficeSupplyRequestSummaryParam
    {
        public int year { get; set; }
        public int month { get; set; }
        public string? keyword { get; set; } = "";
        public int departmentId { get; set; } = 0;
    }
}