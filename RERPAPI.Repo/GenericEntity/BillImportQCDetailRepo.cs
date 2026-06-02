using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillImportQCDetailRepo : GenericRepo<BillImportQCDetail>
    {
        public BillImportQCDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}