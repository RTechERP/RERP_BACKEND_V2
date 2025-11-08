using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class VehicleCategoryRepo : GenericRepo<VehicleCategory>
    {
        public VehicleCategoryRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
