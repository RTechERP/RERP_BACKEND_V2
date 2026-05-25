using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTypeApplicationLinkRepo : GenericRepo<ProjectTypeApplicationLink>
    {
        public ProjectTypeApplicationLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
