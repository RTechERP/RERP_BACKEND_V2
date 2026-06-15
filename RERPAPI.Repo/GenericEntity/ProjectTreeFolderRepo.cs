using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTreeFolderRepo : GenericRepo<ProjectTreeFolder>
    {
        public ProjectTreeFolderRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}