using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskEmployeeRepo : GenericRepo<ProjectTaskEmployee>
    {
        public ProjectTaskEmployeeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}