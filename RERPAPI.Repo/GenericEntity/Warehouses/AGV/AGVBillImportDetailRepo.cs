using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Warehouses.AGV
{
    public class AGVBillImportDetailRepo : GenericRepo<AGVBillImportDetail>
    {
        public AGVBillImportDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}