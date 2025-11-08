using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectSolutionFileRepo : GenericRepo<ProjectSolutionFile>
    {
        public ProjectSolutionFileRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}