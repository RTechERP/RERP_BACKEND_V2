using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectStatusDetailRepo : GenericRepo<ProjectStatusDetail>
    {
        public ProjectStatusDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}