using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.AddNewBillExport
{
    public class DocumentExportRepo : GenericRepo<DocumentExport>
    {
        public DocumentExportRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
