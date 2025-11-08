using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class BillDocumentExportRepo : GenericRepo<BillDocumentExport>
    {
        public BillDocumentExportRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
