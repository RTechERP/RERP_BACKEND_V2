using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepCheckListDetailRepo : GenericRepo<ProjectGateStepCheckListDetail>
    {
        public ProjectGateStepCheckListDetailRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
