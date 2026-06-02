using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.ProductProtectiveGear
{
    public class BillImportDetailTechnicalRepo : GenericRepo<BillImportDetailTechnical>
    {
        public BillImportDetailTechnicalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}