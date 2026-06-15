using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.HRRecruitmentInterviewAssessment
{
    public class HRRecruitmentApproveRepo : GenericRepo<HRRecruitmentApprove>
    {
        public HRRecruitmentApproveRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}