using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartlistPriceRequestTypeRepo : GenericRepo<ProjectPartlistPriceRequestType>
    {
        public ProjectPartlistPriceRequestTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
