using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class VehicleRepairHistoryFileRepo : GenericRepo<VehicleRepairHistoryFile>
    {
        public VehicleRepairHistoryFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
