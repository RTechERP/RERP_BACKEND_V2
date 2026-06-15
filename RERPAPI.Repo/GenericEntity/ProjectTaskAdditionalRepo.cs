using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTaskAdditionalRepo : GenericRepo<ProjectTaskAdditional>
    {
        public ProjectTaskAdditionalRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}