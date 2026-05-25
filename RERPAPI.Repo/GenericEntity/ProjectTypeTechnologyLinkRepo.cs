using RERPAPI.Model.Common;
using RERPAPI.Model.Entities;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTypeTechnologyLinkRepo : GenericRepo<ProjectTypeTechnologyLink>
    {
        public ProjectTypeTechnologyLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
