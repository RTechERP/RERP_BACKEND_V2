using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskChecklistRepo : GenericRepo<ProjectTaskChecklist>
    {
        public ProjectTaskChecklistRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}