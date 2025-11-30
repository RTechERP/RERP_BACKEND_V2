using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillDocumentImportTechnicalRepo : GenericRepo<BillDocumentImportTechnical>
    {
        public BillDocumentImportTechnicalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
