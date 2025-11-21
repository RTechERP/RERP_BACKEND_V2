using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProjectPartlistPurchaseRequestTypeRepo : GenericRepo<ProjectPartlistPurchaseRequestType>
    {
        public ProjectPartlistPurchaseRequestTypeRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}
