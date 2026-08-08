using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepPositionRepo : GenericRepo<ProjectGateStepPosition>
    {
        public ProjectGateStepPositionRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
