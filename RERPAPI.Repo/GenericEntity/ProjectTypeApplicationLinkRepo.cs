using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTypeApplicationLinkRepo : GenericRepo<ProjectTypeApplicationLink>
    {
        public ProjectTypeApplicationLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}