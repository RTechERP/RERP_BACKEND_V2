using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskWorkRepo : GenericRepo<Model.Entities.ProjectTaskWork>
    {
        public ProjectTaskWorkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}