namespace RERPAPI.Model.Param
{
    public class InventoryPram
    {
        public int productGroupID { get; set; } = -1;
        public bool checkAll { get; set; }
        public string Find { get; set; }
        public string WarehouseCode { get; set; }
        public bool IsStock { get; set; }
        //public int PageSize { get; set; }
        //public int PageNumber { get; set; }
    }
}
