using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.HRRecruitmentInterviewAssessment
{
    public class PerformanceCriteriaRepo : GenericRepo<PerformanceCriterion>
    {
        public PerformanceCriteriaRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}