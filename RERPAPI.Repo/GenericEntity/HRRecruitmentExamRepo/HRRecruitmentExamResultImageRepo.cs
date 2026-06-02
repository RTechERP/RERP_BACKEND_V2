using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentExamResultImageRepo : GenericRepo<HRRecruitmentExamResultImage>
    {
        public HRRecruitmentExamResultImageRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}