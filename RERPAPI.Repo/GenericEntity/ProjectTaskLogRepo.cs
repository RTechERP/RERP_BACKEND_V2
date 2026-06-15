using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskLogRepo : GenericRepo<Model.Entities.ProjectTaskLog>
    {
        public ProjectTaskLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}