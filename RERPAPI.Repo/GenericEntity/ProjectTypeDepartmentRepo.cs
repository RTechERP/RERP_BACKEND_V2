using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTypeDepartmentRepo : GenericRepo<ProjectTypeDepartment>
    {
        public ProjectTypeDepartmentRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
