using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.HRRecruitmentInterviewAssessment
{
    public class HRRecruitmentInterviewAssessmentFormRepo : GenericRepo<HRRecruitmentInterviewAssessmentForm>
    {
        public HRRecruitmentInterviewAssessmentFormRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}