using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPriorityRepo : GenericRepo<ProjectPriority>
    {
        public ProjectPriorityRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}