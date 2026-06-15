using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectUserRepo : GenericRepo<ProjectUser>
    {
        public ProjectUserRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}