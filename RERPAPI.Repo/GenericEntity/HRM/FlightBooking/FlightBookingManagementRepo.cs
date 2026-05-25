using RERPAPI.Model.Entities;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity.HRM.FlightBooking
{
    public class FlightBookingManagementRepo : GenericRepo<FlightBookingManagement>
    {
        public FlightBookingManagementRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
