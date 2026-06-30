using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class VehicleRentalRequestRepo : GenericRepo<VehicleRentalRequest>
    {
        public VehicleRentalRequestRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
