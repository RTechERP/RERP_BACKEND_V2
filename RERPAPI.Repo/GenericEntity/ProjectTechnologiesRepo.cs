using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Project
{
    public class ProjectTechnologiesRepo : GenericRepo<ProjectTechnology>
    {
        public ProjectTechnologiesRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}