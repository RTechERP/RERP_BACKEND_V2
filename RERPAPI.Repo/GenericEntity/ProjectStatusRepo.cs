using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectStatusRepo : GenericRepo<ProjectStatus>
    {
        public ProjectStatusRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}