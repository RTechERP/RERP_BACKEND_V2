using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepWorkerRepo : GenericRepo<ProjectGateStepWorker>
    {
        public ProjectGateStepWorkerRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
