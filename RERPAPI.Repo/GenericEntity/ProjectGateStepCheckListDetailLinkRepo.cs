using RERPAPI.Model.Common;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepCheckListDetailLinkRepo : GenericRepo<ProjectGateStepCheckListDetailLink>
    {
        public ProjectGateStepCheckListDetailLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
