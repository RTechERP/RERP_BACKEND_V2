using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Technical
{
    public class BillExportDetailTechnicalRepo : GenericRepo<BillExportDetailTechnical>
    {
        public BillExportDetailTechnicalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}