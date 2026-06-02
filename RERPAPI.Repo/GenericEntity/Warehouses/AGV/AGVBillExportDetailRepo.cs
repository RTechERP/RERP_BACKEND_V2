using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Warehouses.AGV
{
    public class AGVBillExportDetailRepo : GenericRepo<AGVBillExportDetail>
    {
        public AGVBillExportDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}