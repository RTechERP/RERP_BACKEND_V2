using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTypeTechnologyLinkRepo : GenericRepo<ProjectTypeTechnologyLink>
    {
        public ProjectTypeTechnologyLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}