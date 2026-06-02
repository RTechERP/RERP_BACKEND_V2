namespace RERPAPI.Model.DTO
{
    public class ValidateKeepDTO
    {
        public int WarehouseID { get; set; }
        public int ProductID { get; set; }
        public int ProjectID { get; set; }
        public int POKHDetailID { get; set; }
        public int BillExportDetailID { get; set; }
        public decimal QuantityRequestExport { get; set; }

        public string ProductNewCode { get; set; }
        public string UnitName { get; set; }
    }
}