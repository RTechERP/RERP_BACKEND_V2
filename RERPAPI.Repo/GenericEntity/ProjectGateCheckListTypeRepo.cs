using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectGateCheckListTypeRepo : GenericRepo<ProjectGateCheckListType>
    {
        public ProjectGateCheckListTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
