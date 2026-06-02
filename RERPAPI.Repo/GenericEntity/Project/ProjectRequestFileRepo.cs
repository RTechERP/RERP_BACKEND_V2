using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Project
{
    public class ProjectRequestFileRepo : GenericRepo<ProjectRequestFile>
    {
        public ProjectRequestFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}