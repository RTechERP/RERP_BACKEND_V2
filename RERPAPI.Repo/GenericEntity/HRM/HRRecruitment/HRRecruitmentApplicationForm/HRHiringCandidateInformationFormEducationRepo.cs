using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class HRHiringCandidateInformationFormEducationRepo : GenericRepo<HRHiringCandidateInformationFormEducation>
    {
        public HRHiringCandidateInformationFormEducationRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}