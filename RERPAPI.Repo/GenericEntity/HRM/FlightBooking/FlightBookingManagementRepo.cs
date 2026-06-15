using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.FlightBooking
{
    public class FlightBookingManagementRepo : GenericRepo<FlightBookingManagement>
    {
        public FlightBookingManagementRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}