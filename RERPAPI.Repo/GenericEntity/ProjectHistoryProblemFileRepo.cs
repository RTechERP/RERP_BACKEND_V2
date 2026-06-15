using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectHistoryProblemFileRepo : GenericRepo<ProjectHistoryProblemFile>
    {
        public ProjectHistoryProblemFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}