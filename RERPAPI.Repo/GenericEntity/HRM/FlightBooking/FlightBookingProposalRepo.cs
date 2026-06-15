using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.FlightBooking
{
    public class FlightBookingProposalRepo : GenericRepo<FlightBookingProposal>
    {
        public FlightBookingProposalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}