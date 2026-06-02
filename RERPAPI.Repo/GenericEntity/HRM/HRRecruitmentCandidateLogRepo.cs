using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class HRRecruitmentCandidateLogRepo : GenericRepo<HRRecruitmentCandidateLog>
    {
        public HRRecruitmentCandidateLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}