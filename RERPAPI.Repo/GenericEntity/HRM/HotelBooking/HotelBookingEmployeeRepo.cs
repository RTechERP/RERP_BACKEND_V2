using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.HotelBooking
{
    public class HotelBookingEmployeeRepo : GenericRepo<HotelBookingEmployee>
    {
        public HotelBookingEmployeeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
