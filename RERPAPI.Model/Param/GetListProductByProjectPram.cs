namespace RERPAPI.Model.Param
{
    public class GetListProductByProjectPram
    {
        public int projectID { get; set; } = 0;
        public string projectCode { get; set; } = "";
        public string WarehouseCode { get; set; } = "";
        public int CustomerID { get; set; } = 0;
    }
}