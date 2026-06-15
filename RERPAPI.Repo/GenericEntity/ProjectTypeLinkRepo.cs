using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTypeLinkRepo : GenericRepo<ProjectTypeLink>
    {
        public ProjectTypeLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}