using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class InventoryProjectExportRepo : GenericRepo<InventoryProjectExport>
    {
        public InventoryProjectExportRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
