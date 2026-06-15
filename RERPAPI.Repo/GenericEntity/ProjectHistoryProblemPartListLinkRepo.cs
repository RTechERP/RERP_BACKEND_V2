using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectHistoryProblemPartListLinkRepo : GenericRepo<ProjectHistoryProblemPartListLink>
    {
        public ProjectHistoryProblemPartListLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}