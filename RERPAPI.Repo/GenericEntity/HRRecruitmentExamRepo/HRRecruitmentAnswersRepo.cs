using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentAnswersRepo : GenericRepo<HRRecruitmentAnswer>
    {
        public HRRecruitmentAnswersRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}