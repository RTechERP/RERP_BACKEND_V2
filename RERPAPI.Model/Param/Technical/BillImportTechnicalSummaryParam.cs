namespace RERPAPI.Model.Param.Technical
{
    public class BillImportTechnicalSummaryParam
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        public bool IsAll { get; set; }

        public int Status { get; set; }   // = Status trong SP
        public string? FilterText { get; set; }

        public int WarehouseId { get; set; }
    }
}
