using RERPAPI.Model.Entities;
using RERPAPI.Model.DTO;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateStepCheckListRepo : GenericRepo<ProjectGateStepCheckList>
    {
        public ProjectGateStepCheckListRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
