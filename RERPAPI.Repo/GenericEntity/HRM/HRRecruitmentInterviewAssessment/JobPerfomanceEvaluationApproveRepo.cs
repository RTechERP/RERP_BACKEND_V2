using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM.HRRecruitmentInterviewAssessment
{
    public class JobPerfomanceEvaluationApproveRepo : GenericRepo<JobPerfomanceEvaluationApprove>
    {
        public JobPerfomanceEvaluationApproveRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}