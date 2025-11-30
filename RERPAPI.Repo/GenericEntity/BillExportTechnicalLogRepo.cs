using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class BillExportTechnicalLogRepo : GenericRepo<BillExportTechnicalLog>
    {
        public BillExportTechnicalLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
