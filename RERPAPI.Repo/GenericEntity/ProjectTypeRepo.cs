using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTypeRepo : GenericRepo<ProjectType>
    {
        public ProjectTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}