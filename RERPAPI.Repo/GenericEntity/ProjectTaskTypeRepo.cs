using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskTypeRepo : GenericRepo<Model.Entities.ProjectTaskType>
    {
        public ProjectTaskTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}