using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class HRRecruitmentApplicationFormRepo : GenericRepo<HRRecruitmentApplicationForm>
    {
        public HRRecruitmentApplicationFormRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}