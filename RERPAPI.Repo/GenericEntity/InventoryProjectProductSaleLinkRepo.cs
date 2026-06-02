using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class InventoryProjectProductSaleLinkRepo : GenericRepo<InventoryProjectProductSaleLink>
    {
        public InventoryProjectProductSaleLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}