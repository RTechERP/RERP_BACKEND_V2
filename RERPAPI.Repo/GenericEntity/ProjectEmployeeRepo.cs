using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectEmployeeRepo : GenericRepo<ProjectEmployee>
    {
        public ProjectEmployeeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}