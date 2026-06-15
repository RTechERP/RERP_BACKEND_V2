using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectHistoryProblemDetailRepo : GenericRepo<ProjectHistoryProblemDetail>
    {
        public ProjectHistoryProblemDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}