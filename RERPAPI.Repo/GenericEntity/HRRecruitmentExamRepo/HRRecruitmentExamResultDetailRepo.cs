using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRRecruitmentExamRepo
{
    public class HRRecruitmentExamResultDetailRepo : GenericRepo<HRRecruitmentExamResultDetail>
    {
        public HRRecruitmentExamResultDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}