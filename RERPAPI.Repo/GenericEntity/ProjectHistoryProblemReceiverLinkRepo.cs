using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectHistoryProblemReceiverLinkRepo : GenericRepo<ProjectHistoryProblemReceiverLink>
    {
        public ProjectHistoryProblemReceiverLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}