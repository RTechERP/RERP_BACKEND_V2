using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectTypeBaseRepo : GenericRepo<ProjectTypeBase>
    {
        public ProjectTypeBaseRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}