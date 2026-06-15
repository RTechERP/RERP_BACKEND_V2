namespace RERPAPI.Model.DTO
{
    public class ImportExcelRtcDto
    {
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal Quantity { get; set; }
        public string UnitName { get; set; } = "";
        public string Maker { get; set; } = "";
        public string? Note { get; set; }
        public int ProductGroupRTCID { get; set; }
        public int SupplierSaleID { get; set; }
        public DateTime? DateReturnExpected { get; set; }
        public int TicketType { get; set; }
        public bool IsTechBought { get; set; }
        public int ProjectPartListID { get; set; }
        public int WarehouseID { get; set; }
    }
}