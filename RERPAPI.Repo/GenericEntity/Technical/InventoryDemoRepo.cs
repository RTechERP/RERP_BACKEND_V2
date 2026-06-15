using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class InventoryDemoRepo : GenericRepo<InventoryDemo>
    {
        public InventoryDemoRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}