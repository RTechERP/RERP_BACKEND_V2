using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class PONCCHistoryDTO : PONCCHistory
    {
        public int? SupplierSaleID { get; set; }
        public List<int>? lsDeleted { get; set; }
    }
}