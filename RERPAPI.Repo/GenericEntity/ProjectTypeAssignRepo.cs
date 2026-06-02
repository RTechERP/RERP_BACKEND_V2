using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTypeAssignRepo : GenericRepo<ProjectTypeAssign>
    {
        public ProjectTypeAssignRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}