namespace RERPAPI.Model.Param
{
    public class InventoryBorrowNCCParam
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string FilterText { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int WareHouseID { get; set; }
        public int SupplierSaleID { get; set; }
    }
}