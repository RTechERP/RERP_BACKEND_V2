using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectWorkerTypeRepo : GenericRepo<ProjectWorkerType>
    {
        public ProjectWorkerTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}