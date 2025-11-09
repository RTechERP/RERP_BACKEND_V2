using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillDocumentImportLogRepo : GenericRepo<BillDocumentImportLog>
    {
        public BillDocumentImportLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
