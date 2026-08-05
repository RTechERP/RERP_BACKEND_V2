using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.FlightBooking
{
    public class FlightBookingPassengerRepo : GenericRepo<FlightBookingPassenger>
    {
        public FlightBookingPassengerRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
