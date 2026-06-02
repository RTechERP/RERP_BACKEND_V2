using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class InventoryStockRepo : GenericRepo<InventoryStock>
    {
        public InventoryStockRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}