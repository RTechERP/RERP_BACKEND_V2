using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPriorityLinkRepo : GenericRepo<ProjectPriorityLink>
    {
        public ProjectPriorityLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}