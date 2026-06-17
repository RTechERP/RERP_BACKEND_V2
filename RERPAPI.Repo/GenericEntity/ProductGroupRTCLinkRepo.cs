using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Repo.GenericEntity
{
    public class ProductGroupRTCLinkRepo : GenericRepo<ProductGroupRTCLink>
    {
        public ProductGroupRTCLinkRepo(CurrentUser currentUser) : base(currentUser)
        {
        }
    }
}