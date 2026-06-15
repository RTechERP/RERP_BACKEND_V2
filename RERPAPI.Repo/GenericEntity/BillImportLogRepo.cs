using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillImportLogRepo : GenericRepo<BillImportLog>
    {
        public BillImportLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}