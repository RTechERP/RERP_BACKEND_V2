using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillDocumentImportTechnicalLogRepo : GenericRepo<BillDocumentImportTechnicalLog>
    {
        public BillDocumentImportTechnicalLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
