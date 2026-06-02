using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class HRHiringCandidateInformationFormForeignLanguageSkillsRepo : GenericRepo<HRHiringCandidateInformationFormForeignLanguageSkill>
    {
        public HRHiringCandidateInformationFormForeignLanguageSkillsRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}