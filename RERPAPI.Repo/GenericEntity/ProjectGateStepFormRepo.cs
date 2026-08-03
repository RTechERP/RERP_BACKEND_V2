using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepFormRepo : GenericRepo<ProjectGateStepForm>
    {
        public ProjectGateStepFormRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
