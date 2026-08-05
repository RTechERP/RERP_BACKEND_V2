using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.HotelBooking
{
    public class HotelBookingProposalRepo : GenericRepo<HotelBookingProposal>
    {
        public HotelBookingProposalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
