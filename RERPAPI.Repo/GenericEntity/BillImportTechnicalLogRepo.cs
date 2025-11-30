using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillImportTechnicalLogRepo : GenericRepo<BillImportTechnicalLog>
    {
        public BillImportTechnicalLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
