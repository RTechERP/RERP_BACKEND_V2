using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepTemplateRepo : GenericRepo<ProjectGateStepTemplate>
    {
        public ProjectGateStepTemplateRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
