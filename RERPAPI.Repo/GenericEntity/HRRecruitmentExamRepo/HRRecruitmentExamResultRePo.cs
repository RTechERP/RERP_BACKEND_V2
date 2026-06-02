using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentExamResultRepo : GenericRepo<HRRecruitmentExamResult>
    {
        public HRRecruitmentExamResultRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}