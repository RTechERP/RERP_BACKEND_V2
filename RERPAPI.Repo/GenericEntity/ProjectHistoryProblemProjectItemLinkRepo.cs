using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectHistoryProblemProjectItemLinkRepo : GenericRepo<ProjectHistoryProblemProjectItemLink>
    {
        public ProjectHistoryProblemProjectItemLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}