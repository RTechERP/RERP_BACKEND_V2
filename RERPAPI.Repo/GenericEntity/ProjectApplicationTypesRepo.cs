using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity.Project
{
    public class ProjectApplicationTypesRepo : GenericRepo<ProjectApplicationType>
    {
        public ProjectApplicationTypesRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}