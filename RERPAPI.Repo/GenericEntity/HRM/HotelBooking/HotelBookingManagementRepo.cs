using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.HotelBooking
{
    public class HotelBookingManagementRepo : GenericRepo<HotelBookingManagement>
    {
        public HotelBookingManagementRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
