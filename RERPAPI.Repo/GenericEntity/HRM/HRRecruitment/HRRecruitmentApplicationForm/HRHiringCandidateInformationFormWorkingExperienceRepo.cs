using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class HRHiringCandidateInformationFormWorkingExperienceRepo : GenericRepo<HRHiringCandidateInformationFormWorkingExperience>
    {
        public HRHiringCandidateInformationFormWorkingExperienceRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}