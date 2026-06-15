using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class HRHiringCandidateInformationEmergencyContactRepo : GenericRepo<HRHiringCandidateInformationEmergencyContact>
    {
        public HRHiringCandidateInformationEmergencyContactRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}