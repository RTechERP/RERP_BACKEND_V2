using RERPAPI.Model.Entities;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity.HRM.FlightBooking
{
    public class FlightBookingProposalRepo : GenericRepo<FlightBookingProposal>
    {
        public FlightBookingProposalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
