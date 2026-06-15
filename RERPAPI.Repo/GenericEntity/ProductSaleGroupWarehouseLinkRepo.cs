using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductSaleGroupWarehouseLinkRepo : GenericRepo<ProductSaleGroupWarehouseLink>
    {
        public ProductSaleGroupWarehouseLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}