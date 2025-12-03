namespace RERPAPI.Model.Param.Technical
{
    public class GetInventoryBorrowSupplierDemoRequestParam
    {
        public int? SupplierDemoID { get; set; }
        public int? WarehouseID { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string? FilterText { get; set; }
        public int? Page { get; set; }
        public int? Size { get; set; }
        public int? WarehouseType { get; set; }
    }
}
