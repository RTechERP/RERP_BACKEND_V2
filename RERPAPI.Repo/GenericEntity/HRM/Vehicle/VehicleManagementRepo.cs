using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class VehicleManagementRepo : GenericRepo<VehicleManagement>
    {
        public VehicleManagementRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
