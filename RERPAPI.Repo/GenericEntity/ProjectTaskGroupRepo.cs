using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskGroupRepo : GenericRepo<ProjectTaskGroup>
    {
        public ProjectTaskGroupRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}