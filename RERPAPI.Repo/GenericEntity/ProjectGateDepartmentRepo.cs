using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateDepartmentRepo : GenericRepo<ProjectGateDepartment>
    {
        public ProjectGateDepartmentRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
