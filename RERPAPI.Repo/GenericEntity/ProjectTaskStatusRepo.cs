using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskStatusRepo : GenericRepo<Model.Entities.ProjectTaskStatus>
    {
        public ProjectTaskStatusRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}