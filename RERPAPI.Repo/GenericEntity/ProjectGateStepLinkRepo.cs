using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepLinkRepo : GenericRepo<ProjectGateStepLink>
    {
        public ProjectGateStepLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
