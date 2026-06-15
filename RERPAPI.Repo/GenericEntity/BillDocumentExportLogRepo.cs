using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillDocumentExportLogRepo : GenericRepo<BillDocumentExportLog>
    {
        public BillDocumentExportLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}