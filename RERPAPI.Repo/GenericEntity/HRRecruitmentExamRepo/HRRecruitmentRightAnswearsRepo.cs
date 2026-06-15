using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentRightAnswearsRepo : GenericRepo<HRRecruitmentRightAnswer>
    {
        public HRRecruitmentRightAnswearsRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}