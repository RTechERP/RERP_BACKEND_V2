using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Project
{
    public class ProjectItemFileRepo : GenericRepo<ProjectItemFile>
    {
        public ProjectItemFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}