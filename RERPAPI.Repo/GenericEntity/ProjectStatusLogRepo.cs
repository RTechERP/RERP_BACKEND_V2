using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectStatusLogRepo : GenericRepo<ProjectStatusLog>
    {
        public ProjectStatusLogRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}