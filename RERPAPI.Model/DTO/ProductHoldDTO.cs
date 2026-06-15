using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProductHoldDTO : InventoryProject
    {
        public List<int> ProjectParlistPurchaseRequestID { get; set; }
    }
}