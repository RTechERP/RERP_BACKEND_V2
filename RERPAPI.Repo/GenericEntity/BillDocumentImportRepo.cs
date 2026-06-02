using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillDocumentImportRepo : GenericRepo<BillDocumentImport>
    {
        public BillDocumentImportRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}