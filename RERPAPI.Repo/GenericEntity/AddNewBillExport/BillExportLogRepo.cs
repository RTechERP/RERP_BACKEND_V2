using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class BillExportLogRepo : GenericRepo<BillExportLog>
    {
        public BillExportLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
