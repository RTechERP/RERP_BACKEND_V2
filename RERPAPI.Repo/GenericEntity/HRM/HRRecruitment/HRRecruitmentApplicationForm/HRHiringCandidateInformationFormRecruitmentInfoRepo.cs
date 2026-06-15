using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class HRHiringCandidateInformationFormRecruitmentInfoRepo : GenericRepo<HRHiringCandidateInformationFormRecruitmentInfo>
    {
        public HRHiringCandidateInformationFormRecruitmentInfoRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}