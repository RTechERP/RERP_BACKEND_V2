using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.Vehicle
{
    public class VehicleBookingFileRepo : GenericRepo<VehicleBookingFile>
    {
        public VehicleBookingFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}