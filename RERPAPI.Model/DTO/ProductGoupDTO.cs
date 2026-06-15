using RERPAPI.Model.Entities;

namespace RERPAPI.Model.DTO
{
    public class ProductGoupDTO
    {
        public ProductGroup Productgroup { get; set; }
        public ProductGroupWarehouse? ProductgroupWarehouse { get; set; }
    }
}