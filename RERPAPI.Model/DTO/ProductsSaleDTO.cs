using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProductsSaleDTO
    {
        public ProductSale ProductSale { get; set; }
        public Inventory? Inventory { get; set; }
    }
}