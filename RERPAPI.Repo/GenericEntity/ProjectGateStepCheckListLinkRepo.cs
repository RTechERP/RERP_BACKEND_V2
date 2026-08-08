using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepCheckListLinkRepo : GenericRepo<ProjectGateStepCheckListLink>
    {
        public ProjectGateStepCheckListLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
