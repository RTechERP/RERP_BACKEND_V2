using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentQuestionImageRepo : GenericRepo<HRRecruitmentQuestionImage>
    {
        public HRRecruitmentQuestionImageRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}