using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class JobPerfomanceEvaluationNewLogRepo : GenericRepo<JobPerfomanceEvaluationNewLog>
    {
        public JobPerfomanceEvaluationNewLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}