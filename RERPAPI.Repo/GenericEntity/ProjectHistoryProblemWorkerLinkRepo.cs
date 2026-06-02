using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectHistoryProblemWorkerLinkRepo : GenericRepo<ProjectHistoryProblemWorkerLink>
    {
        public ProjectHistoryProblemWorkerLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}