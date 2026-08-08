using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepFileRepo : GenericRepo<ProjectGateStepFile>
    {
        public ProjectGateStepFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
