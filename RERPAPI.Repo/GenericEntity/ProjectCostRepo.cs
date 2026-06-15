using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectCostRepo : GenericRepo<ProjectCost>
    {
        public ProjectCostRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}