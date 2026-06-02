using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.HRM
{
    public class JobPerfomanceEvaluationNewRepo : GenericRepo<JobPerfomanceEvaluationNew>
    {
        public JobPerfomanceEvaluationNewRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}