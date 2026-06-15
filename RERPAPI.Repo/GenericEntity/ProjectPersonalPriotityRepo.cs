using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPersonalPriotityRepo : GenericRepo<ProjectPersonalPriotity>
    {
        public ProjectPersonalPriotityRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}