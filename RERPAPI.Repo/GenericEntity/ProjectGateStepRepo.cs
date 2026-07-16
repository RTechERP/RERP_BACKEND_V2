using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepRepo : GenericRepo<ProjectGateStep>
    {
        public ProjectGateStepRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
